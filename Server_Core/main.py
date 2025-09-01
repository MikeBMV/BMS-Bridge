# File: Server_Core/main.py
import asyncio
import logging
import uvicorn
import sys
import os
import multiprocessing
import time
from contextlib import asynccontextmanager
from pathlib import Path
from typing import List, Dict, Any, Optional

import structlog
from fastapi import FastAPI, HTTPException, WebSocket, WebSocketDisconnect, Depends, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse, JSONResponse
from pydantic import BaseModel

import socket
from enum import Enum

class ServerStatus(str, Enum):
    RUNNING = "RUNNING"
    WARNING = "WARNING"
    ERROR = "ERROR" # A placeholder for future use

class BmsStatus(str, Enum):
    CONNECTED = "CONNECTED"
    NOT_CONNECTED = "NOT_CONNECTED"

def get_server_ip() -> str:
    """Returns the local network IP address of the server."""
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        # Connect to an external address to find out our local IP
        s.connect(("8.8.8.8", 80)) 
        ip = s.getsockname()[0]
        s.close()
        return ip
    except Exception:
        return "127.0.0.1"

# --- 1. Configure logging using structlog ---
structlog.configure(
    processors=[
        structlog.processors.add_log_level,
        structlog.processors.TimeStamper(fmt="iso"),
        structlog.processors.StackInfoRenderer(),
        structlog.dev.ConsoleRenderer(), # For pretty console logs
    ],
    logger_factory=structlog.PrintLoggerFactory(),
)
logger = structlog.get_logger()

# Configure standard logging to write to a file
log_file_path = Path(__file__).resolve().parent / "bms_bridge.log"
logging.basicConfig(
    level=logging.DEBUG,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[logging.FileHandler(log_file_path, mode='w')]
)

# --- 2. Define the base path for resources ---
def get_base_path():
    # Determines the base directory for the app, works for both frozen (PyInstaller) and normal modes
    if getattr(sys, 'frozen', False):
        return Path(sys.executable).parent.resolve()
    else:
        return Path(__file__).resolve().parent
BASE_DIR = get_base_path()

# --- Import our application modules ---
from config.settings import ConfigManager
from adapters.bms_adapter import BMSAdapter
from services.briefing_service import BriefingService
from services.kneeboard_service import KneeboardService

# --- 3. Pydantic Models ---
class KneeboardResponse(BaseModel):
    success: bool
    images: List[str] = []
    error: str = ""
    cached: bool = False

# --- 4. Helper classes (RateLimiter, WebSocketManager) ---

class RateLimiter:
    """A simple request rate limiter."""
    def __init__(self, requests_per_minute: int):
        self.requests_per_minute = requests_per_minute
        self.requests: Dict[str, List[float]] = {}
    
    def is_allowed(self, client_id: str) -> bool:
        now = time.time()
        client_requests = self.requests.setdefault(client_id, [])
        # Remove old requests
        client_requests[:] = [t for t in client_requests if now - t < 60]
        if len(client_requests) >= self.requests_per_minute:
            return False
        client_requests.append(now)
        return True

class WebSocketManager:
    """Manages active WebSocket connections."""
    def __init__(self, max_connections: int):
        self.max_connections = max_connections
        self.active_connections: List[WebSocket] = []
    
    async def connect(self, websocket: WebSocket) -> bool:
        if len(self.active_connections) >= self.max_connections:
            await websocket.close(code=1008, reason="Too many connections")
            return False
        await websocket.accept()
        self.active_connections.append(websocket)
        logger.info("WebSocket connected", count=len(self.active_connections))
        return True
    
    def disconnect(self, websocket: WebSocket):
        if websocket in self.active_connections:
            self.active_connections.remove(websocket)
            logger.info("WebSocket disconnected", count=len(self.active_connections))

# --- 5. Main application class for Dependency Injection ---

class BMSBridgeApp:
    """
    Main application class that holds all services and managers.
    This allows for a clean, global-variable-free architecture using FastAPI's dependency injection.
    """
    def __init__(self, base_dir: Path):
        self.base_dir = base_dir
        self.start_time = time.time()
        
        self.config_manager = ConfigManager(base_dir)
        self.config = self.config_manager.load_config()
        self.security_config = self.config_manager.get_security_config()
        
        self.bms_adapter = BMSAdapter(
            failure_threshold=self.config.circuit_breaker_failure_threshold,
            reset_timeout=self.config.circuit_breaker_reset_timeout
        )
        self.briefing_service = BriefingService(self.config_manager)
        self.kneeboard_service = KneeboardService(base_dir, self.config_manager)
        
        # In a real-world app, rate_limiter would be used here
        # self.rate_limiter = RateLimiter(...)
        self.websocket_manager = WebSocketManager(self.config.max_websocket_connections)

# --- 6. FastAPI Lifespan and Dependency Injection Setup ---

app_instance: Optional[BMSBridgeApp] = None

@asynccontextmanager
async def lifespan(app: FastAPI):
    """Handles application startup and shutdown events."""
    global app_instance
    logger.info("Application starting up...")
    app_instance = BMSBridgeApp(BASE_DIR)
    yield
    logger.info("Application shutting down...")
    if app_instance:
        app_instance.bms_adapter.close()
    app_instance = None

def get_app() -> BMSBridgeApp:
    """A dependency that provides access to the main application instance."""
    if app_instance is None:
        # This should never happen in a running application
        raise HTTPException(status_code=503, detail="Application is not initialized")
    return app_instance

# --- 7. FastAPI App Creation and Endpoints ---

app = FastAPI(title="BMS Bridge", version="0.1.0", lifespan=lifespan)
app.add_middleware(CORSMiddleware, allow_origins=["*"], allow_credentials=True, allow_methods=["*"], allow_headers=["*"])

@app.get("/api/health")
async def health_check(request: Request, app_inst: BMSBridgeApp = Depends(get_app)):
    """Returns the detailed status of the server and the simulator connection."""
    bms_connected = app_inst.bms_adapter.is_connected()
    
    server_status = ServerStatus.RUNNING if bms_connected else ServerStatus.WARNING
    bms_status = BmsStatus.CONNECTED if bms_connected else BmsStatus.NOT_CONNECTED
    
    # Construct the full server address
    host_ip = get_server_ip()
    port = app_inst.config.server_port
    server_address = f"http://{host_ip}:{port}"

    return {
        "server_status": server_status,
        "bms_status": bms_status,
        "server_address": server_address,
        "server_message": "OK" if bms_connected else "BMS Shared Memory not available. Is the simulator in 3D?"
    }

@app.post("/api/kneeboards/refresh")
async def refresh_kneeboards(app_inst: BMSBridgeApp = Depends(get_app)):
    result = app_inst.kneeboard_service.refresh_kneeboards()
    if not result.get("success"):
        raise HTTPException(status_code=500, detail=result.get("error", "Unknown error"))
    return result

@app.get("/api/kneeboard_images/{board_name}", response_model=KneeboardResponse)
async def get_kneeboard_images(board_name: str, app_inst: BMSBridgeApp = Depends(get_app)):
    if board_name not in ["Left", "Right"]:
        raise HTTPException(status_code=400, detail="Invalid board name")
    image_dir = app_inst.base_dir / board_name
    images = []
    if image_dir.is_dir():
        images = sorted([f.name for f in image_dir.iterdir() if f.is_file() and f.suffix.lower() in app_inst.config.allowed_image_extensions])
    return KneeboardResponse(success=True, images=images)

@app.get("/api/briefing")
async def get_briefing(app_inst: BMSBridgeApp = Depends(get_app)):
    result = app_inst.briefing_service.get_briefing_data(app_inst.bms_adapter)
    if not result.get("success"):
        return JSONResponse(status_code=404, content=result)
    return JSONResponse(content=result)

@app.websocket("/ws/flight_data")
async def websocket_flight_data(websocket: WebSocket, app_inst: BMSBridgeApp = Depends(get_app)):
    if not await app_inst.websocket_manager.connect(websocket):
        return
    
    try:
        while True:
            data = app_inst.bms_adapter.get_all_data()
            if data:
                await websocket.send_json({"success": True, "data": data})
            else:
                await websocket.send_json({"success": False, "error": "No data from BMS"})
            await asyncio.sleep(app_inst.config.websocket_update_interval)
    except WebSocketDisconnect:
        app_inst.websocket_manager.disconnect(websocket)
    except Exception as e:
        logger.error("WebSocket error", error=str(e))
        app_inst.websocket_manager.disconnect(websocket)

@app.get("/{filepath:path}")
async def serve_static_or_app(filepath: str = "", app_inst: BMSBridgeApp = Depends(get_app)):
    """Serves static files or the main index.html for the frontend single-page application."""
    safe_path = app_inst.base_dir / filepath.replace("..", "").strip("/\\")
    if filepath and filepath.split('/')[0] in app_inst.security_config.allowed_static_paths:
        if safe_path.is_file():
            if safe_path.suffix == ".mjs": return FileResponse(safe_path, media_type="application/javascript")
            return FileResponse(safe_path)
    index_file = app_inst.base_dir / "templates" / "index.html"
    if not index_file.exists(): raise HTTPException(status_code=404, detail="index.html not found")
    return FileResponse(index_file)

if __name__ == "__main__":
    # This is required for PyInstaller to work correctly with multiprocessing on Windows
    multiprocessing.freeze_support()
    config = ConfigManager(BASE_DIR).load_config()
    uvicorn.run(app, host=config.server_host, port=config.server_port, workers=1)