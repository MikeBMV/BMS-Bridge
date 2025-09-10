# File: Server_Core/main.py - CORRECTED STARTUP SEQUENCE
import asyncio
import logging
import uvicorn
import sys
import glob
import os
import multiprocessing
import time
from contextlib import asynccontextmanager
from pathlib import Path
from typing import List, Dict, Any, Optional
import socket
from enum import Enum

import argparse
import platform
import ctypes

import structlog
from fastapi import FastAPI, HTTPException, WebSocket, WebSocketDisconnect, Depends, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse, JSONResponse, HTMLResponse
from pydantic import BaseModel

from bs4 import BeautifulSoup

from config.settings import ConfigManager
from adapters.bms_adapter import BMSAdapter
from services.briefing_service import BriefingService
from services.path_service import PathService
# --------------------------------------------------

# --- 0. Command-line argument parsing for hiding the console ---
def hide_console_window_if_requested():
    parser = argparse.ArgumentParser(description="BMS Bridge Server")
    parser.add_argument('--hide-console', action='store_true', help='If specified, the console window will be hidden on startup.')
    args, _ = parser.parse_known_args()
    if args.hide_console and platform.system() == "Windows":
        try:
            hwnd = ctypes.windll.kernel32.GetConsoleWindow()
            if hwnd:
                ctypes.windll.user32.ShowWindow(hwnd, 0)
        except Exception:
            pass

hide_console_window_if_requested()

logger = logging.getLogger(__name__)

def get_base_path():
    if getattr(sys, 'frozen', False):
        return Path(sys.executable).parent.resolve()
    else:
        return Path(__file__).resolve().parent
BASE_DIR = get_base_path()

class ServerStatus(str, Enum): RUNNING = "RUNNING"; WARNING = "WARNING"; ERROR = "ERROR"
class BmsStatus(str, Enum): CONNECTED = "CONNECTED"; NOT_CONNECTED = "NOT_CONNECTED"

def get_server_ip() -> str:
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM); s.connect(("8.8.8.8", 80)); ip = s.getsockname()[0]; s.close(); return ip
    except Exception: return "127.0.0.1"

class KneeboardItemResponse(BaseModel): path: str; type: str
class KneeboardListResponse(BaseModel): success: bool; items: List[KneeboardItemResponse] = []

class WebSocketManager:
    def __init__(self, max_connections: int): self.max_connections = max_connections; self.active_connections: List[WebSocket] = []
    async def connect(self, websocket: WebSocket) -> bool:
        if len(self.active_connections) >= self.max_connections: await websocket.close(code=1008, reason="Too many connections"); return False
        await websocket.accept(); self.active_connections.append(websocket); logger.info("WebSocket connected", count=len(self.active_connections)); return True
    def disconnect(self, websocket: WebSocket):
        if websocket in self.active_connections: self.active_connections.remove(websocket); logger.info("WebSocket disconnected", count=len(self.active_connections))

class BMSBridgeApp:
    def __init__(self, base_dir: Path):
        self.base_dir = base_dir
        self.config_manager = ConfigManager(base_dir)
        self.config = self.config_manager.load_config()
        self.security_config = self.config_manager.get_security_config()
        self.bms_adapter = BMSAdapter(failure_threshold=self.config.circuit_breaker_failure_threshold, reset_timeout=self.config.circuit_breaker_reset_timeout)
        self.briefing_service = BriefingService(self.config_manager)
        self.path_service = PathService()
        self.websocket_manager = WebSocketManager(self.config.max_websocket_connections)

app_instance: Optional[BMSBridgeApp] = None
@asynccontextmanager
async def lifespan(app: FastAPI):
    global app_instance; logger.info("Application starting up..."); app_instance = BMSBridgeApp(BASE_DIR); yield
    logger.info("Application shutting down..."); 
    if app_instance: app_instance.bms_adapter.close()
def get_app() -> BMSBridgeApp:
    if app_instance is None: raise HTTPException(status_code=503, detail="Application is not initialized")
    return app_instance

app = FastAPI(title="BMS Bridge", version="0.2.0", lifespan=lifespan)
app.add_middleware(CORSMiddleware, allow_origins=["*"], allow_credentials=True, allow_methods=["*"], allow_headers=["*"])

@app.get("/api/health")
async def health_check(request: Request, app_inst: BMSBridgeApp = Depends(get_app)):
    flight_data = app_inst.bms_adapter.get_all_data()
    bms_connected = app_inst.bms_adapter.is_connected()
    bms_connected = flight_data is not None
    return {
        "server_status": ServerStatus.RUNNING if bms_connected else ServerStatus.WARNING,
        "bms_status": BmsStatus.CONNECTED if bms_connected else BmsStatus.NOT_CONNECTED,
        "server_address": f"http://{get_server_ip()}:{app_inst.config.server_port}",
        "server_message": "OK" if bms_connected else "BMS Shared Memory not available. Is the simulator in 3D?"
    }

@app.get("/api/kneeboards/{board_name}", response_model=KneeboardListResponse)
async def get_kneeboard_list(board_name: str, app_inst: BMSBridgeApp = Depends(get_app)):
    if board_name not in ["left", "right"]: raise HTTPException(status_code=404, detail="Board not found. Use 'left' or 'right'.")
    kneeboard_config = app_inst.config_manager.load_config().kneeboards
    raw_list = getattr(kneeboard_config, board_name, [])
    response_items = [KneeboardItemResponse(path=f"/user_data/kneeboards/{Path(item.path).name}", type="pdf" if Path(item.path).suffix.lower() == ".pdf" else "image") for item in raw_list if item.enabled]
    return KneeboardListResponse(success=True, items=response_items)

@app.get("/api/briefing")
async def get_briefing(app_inst: BMSBridgeApp = Depends(get_app)):
    result = app_inst.briefing_service.get_briefing_data(app_inst.bms_adapter)
    if not result.get("success"): return JSONResponse(status_code=404, content=result)
    return JSONResponse(content=result)

@app.websocket("/ws/flight_data")
async def websocket_flight_data(websocket: WebSocket, app_inst: BMSBridgeApp = Depends(get_app)):
    if not await app_inst.websocket_manager.connect(websocket): return
    try:
        while True:
            data = app_inst.bms_adapter.get_all_data()
            await websocket.send_json({"success": bool(data), "data": data or None, "error": "No data from BMS" if not data else ""})
            await asyncio.sleep(app_inst.config.websocket_update_interval)
    except WebSocketDisconnect: app_inst.websocket_manager.disconnect(websocket)
    except Exception as e: logger.error(f"WebSocket error: {e}"); app_inst.websocket_manager.disconnect(websocket)

@app.get("/api/briefing/html", response_class=HTMLResponse)
async def get_html_briefing(app_inst: BMSBridgeApp = Depends(get_app)):
    try:
        briefings_dir = app_inst.path_service.find_briefings_dir(app_inst.bms_adapter)
        if not briefings_dir or not briefings_dir.is_dir():
            raise HTTPException(status_code=404, detail="BMS Briefings directory not found. Is Falcon BMS installed?")
        
        search_pattern = str(briefings_dir / "*.html")
        html_files = glob.glob(search_pattern)

        if not html_files:
            logger.warning(f"No HTML briefing files found in: {briefings_dir}")
            raise HTTPException(status_code=404, detail="No HTML briefing files found.")

        latest_briefing_path = max(html_files, key=os.path.getmtime)
        logger.info(f"Serving HTML briefing from: {latest_briefing_path}")

        with open(latest_briefing_path, 'r', encoding='utf-8', errors='ignore') as f:
            soup = BeautifulSoup(f, 'lxml')
        
        body_tag = soup.find('body')
        
        if not body_tag:
            logger.error(f"Could not find <body> tag in briefing file: {latest_briefing_path}")
            raise HTTPException(status_code=500, detail="Could not find <body> tag in briefing file.")
            
        body_content = body_tag.decode_contents()
        
        return HTMLResponse(content=body_content)

    except HTTPException:
        raise
    except Exception:
        logger.error("An unexpected error occurred in get_html_briefing", exc_info=True)
        raise HTTPException(status_code=500, detail="An internal server error occurred while processing the briefing.")

@app.get("/{filepath:path}")
async def serve_static_or_app(filepath: str = "", app_inst: BMSBridgeApp = Depends(get_app)):
    logger.info(f"--- Static File Request Received --- path: {filepath}")
    if not filepath: logger.info("Filepath empty, serving index.html"); return FileResponse(BASE_DIR / "templates" / "index.html")
    try:
        base_dir = BASE_DIR.resolve(); full_path = (base_dir / filepath).resolve()
        logger.info(f"Resolved absolute path: {full_path}")
        if not str(full_path).startswith(str(base_dir)): logger.warning(f"SECURITY: Path traversal attempt! Requested path: {full_path}"); raise HTTPException(status_code=403)
        is_allowed = any(str(full_path).startswith(str((base_dir / p).resolve())) for p in app_inst.security_config.allowed_static_paths)
        if is_allowed and full_path.is_file():
            logger.info(f">>> Serving STATIC FILE: {full_path}")
            return FileResponse(full_path, media_type="application/javascript" if full_path.suffix == ".mjs" else None)
        logger.warning(f"Path not allowed or not a file. Allowed: {is_allowed}, Is file: {full_path.is_file()}")
    except Exception as e: logger.error("Exception in static file serving", exc_info=True)
    logger.warning(">>> Fallback: Serving INDEX.HTML")
    return FileResponse(BASE_DIR / "templates" / "index.html")

    
if __name__ == "__main__":
    multiprocessing.freeze_support()
    config = ConfigManager(BASE_DIR).load_config()
    uvicorn.run(
        app,
        host=config.server_host,
        port=config.server_port,
        workers=1,
        log_config="log_config.yaml"
    )