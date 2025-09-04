# File: config/settings.py
from pathlib import Path
from typing import List, Optional
from pydantic import BaseModel, Field, validator
from pydantic_settings import BaseSettings
import logging

# We will use standard logging, but take inspiration from structlog concepts
logger = logging.getLogger(__name__)

class KneeboardItem(BaseModel):
    path: str
    enabled: bool
    type: str = "user_file"

class KneeboardConfig(BaseModel):
    left: List[KneeboardItem] = []
    right: List[KneeboardItem] = []

# ------------------------------------

class ServerConfig(BaseSettings):
    """Server configuration with validation."""
    server_host: str = Field(default="0.0.0.0", description="Server host")
    server_port: int = Field(default=8000, ge=1024, le=65535, description="Server port")
    allowed_image_extensions: List[str] = Field(default=[".png", ".jpg", ".jpeg", ".webp"])
    websocket_update_interval: float = Field(default=0.1, ge=0.05, le=1.0)
    kneeboard_scale_width: float = Field(default=1.4, ge=0.5, le=3.0, description="Kneeboard width scaling factor (1.4 = 140%)")
    
    max_websocket_connections: int = Field(default=10, ge=1)
    circuit_breaker_failure_threshold: int = Field(default=5, ge=3)
    circuit_breaker_reset_timeout: int = Field(default=60, ge=30, description="Seconds before retrying a broken connection")
    file_cache_ttl_seconds: int = Field(default=300, ge=60, description="Time to cache briefing and kneeboard data")

    kneeboards: KneeboardConfig = Field(default_factory=KneeboardConfig)

    class Config:
        model_config = {
            "json_nested": True
        }
        env_prefix = "BMS_"

class SecurityConfig(BaseModel):
    """Security-related configuration."""
    allowed_static_paths: List[str] = Field(default=["static", "libs", "images", "procedure", "Left", "Right", "user_data"])
    
    @validator('allowed_static_paths')
    def validate_static_paths(cls, v):
        for path in v:
            if '..' in path or path.startswith('/') or '\\' in path:
                raise ValueError(f"Invalid characters in static path: {path}")
        return v

class ConfigManager:
    """Manages loading and saving of application configuration."""
    def __init__(self, base_dir: Path):
        self.config_file = base_dir / "config" / "settings.json"
        self.security_config_file = base_dir / "config" / "security.json"
        self._config: Optional[ServerConfig] = None
        self._security_config: Optional[SecurityConfig] = None

    def load_config(self) -> ServerConfig:
        if self._config:
            return self._config
        try:
            if self.config_file.exists():
                self._config = ServerConfig.model_validate_json(self.config_file.read_text())
            else:
                self._config = ServerConfig()
                self.save_config()
            return self._config
        except Exception as e:
            logger.error(f"Failed to load config, using defaults: {e}")
            return ServerConfig()

    def save_config(self):
        if not self._config: return
        try:
            self.config_file.parent.mkdir(parents=True, exist_ok=True)
            self.config_file.write_text(self._config.model_dump_json(indent=2))
        except Exception as e:
            logger.error(f"Failed to save config: {e}")


    def get_security_config(self) -> SecurityConfig:
        if self._security_config:
            return self._security_config
        try:
            if self.security_config_file.exists():
                self._security_config = SecurityConfig.model_validate_json(self.security_config_file.read_text())
            else:
                self._security_config = SecurityConfig()
                self.security_config_file.write_text(self._security_config.model_dump_json(indent=2))
        except Exception as e:
            logger.error(f"Failed to load security config: {e}")
            self._security_config = SecurityConfig()
        return self._security_config