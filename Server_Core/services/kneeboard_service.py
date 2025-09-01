# File: services/kneeboard_service.py
import os
import winreg
import platform
import hashlib
import time
import logging
from pathlib import Path
from PIL import Image
from typing import Optional, Dict, List, Tuple

from config.settings import ConfigManager

logger = logging.getLogger(__name__)

# These values should ideally be moved to the settings.json config file in the future.
BMS_VERSION = "4.37"
WIN_REG_KEY = rf"SOFTWARE\WOW6432Node\Benchmark Sims\Falcon BMS {BMS_VERSION}"
DDS_OBJECTS_DIR = os.path.join("Data", "TerrData", "Objects", "KoreaObj")
INIT_DDS = 7982
DDS_COUNT = 16
OUT_EXT = "png"

class KneeboardService:
    def __init__(self, base_app_dir: Path, config_manager: ConfigManager):
        self.base_app_dir = base_app_dir
        self.config_manager = config_manager
        self.bms_base_dir: Optional[Path] = self._find_bms_base_dir()
        self.dds_dir: Optional[Path] = self.bms_base_dir / DDS_OBJECTS_DIR if self.bms_base_dir else None

    def _find_bms_base_dir(self) -> Optional[Path]:
        # The search logic remains the same
        if platform.system() != "Windows": return None
        try:
            con = winreg.ConnectRegistry(None, winreg.HKEY_LOCAL_MACHINE)
            key = winreg.OpenKey(con, WIN_REG_KEY)
            base_dir_str, _ = winreg.QueryValueEx(key, "baseDir")
            winreg.CloseKey(key)
            if base_dir_str and Path(base_dir_str).is_dir(): return Path(base_dir_str)
        except Exception: pass
        return None

    def _get_dds_files_hash(self) -> Optional[str]:
        """Creates an MD5 hash from the names, sizes, and modification times of all DDS files."""
        if not self.dds_dir or not self.dds_dir.is_dir(): return None
        try:
            file_info = []
            for i in range(INIT_DDS, INIT_DDS + DDS_COUNT):
                dds_file = self.dds_dir / f"{i}.dds"
                if dds_file.exists():
                    stat = dds_file.stat()
                    file_info.append(f"{dds_file.name}:{stat.st_size}:{stat.st_mtime}")
            
            if not file_info: return None
            return hashlib.md5("".join(sorted(file_info)).encode()).hexdigest()
        except Exception as e:
            logger.error(f"Error calculating DDS files hash: {e}")
            return None

    def _write_cropped_png(self, img: Image.Image, dims: tuple, target_path: Path, scale_factor: float):
        # The conversion logic remains the same
        try:
            cropped = img.crop(dims)
            if scale_factor != 1.0:
                original_width, original_height = cropped.size
                new_width = int(original_width * scale_factor)
                final_image = cropped.resize((new_width, original_height), Image.Resampling.LANCZOS)
            else:
                final_image = cropped
            target_path.parent.mkdir(parents=True, exist_ok=True)
            final_image.save(target_path, format=OUT_EXT)
        except Exception as e:
            logger.error(f"Failed to write image {target_path}: {e}")

    def refresh_kneeboards(self) -> dict:
        """Updates the kneeboards, but only if the source DDS files have changed."""
        if not self.dds_dir:
            return {"success": False, "error": "BMS DDS directory not found."}

        # --- 1. Check if an update is needed ---
        cached_paths = self.config_manager.get_cached_paths()
        current_hash = self._get_dds_files_hash()

        if current_hash and current_hash == cached_paths.kneeboard_files_hash:
            logger.info("Kneeboard DDS files have not changed. Skipping conversion.")
            return {"success": True, "message": "Images are already up to date.", "cached": True}

        logger.info("Kneeboard DDS files have changed. Starting conversion...")

        # --- 2. If needed, perform the conversion ---
        config = self.config_manager.load_config()
        scale_factor = config.kneeboard_scale_width
        left_dir, right_dir = self.base_app_dir / "Left", self.base_app_dir / "Right"
        converted_count = 0
        
        for i in range(INIT_DDS, INIT_DDS + DDS_COUNT):
            source_dds = self.dds_dir / f"{i}.dds"
            if not source_dds.is_file(): continue
            try:
                with Image.open(source_dds) as img:
                    width, height = img.size
                    self._write_cropped_png(img, (0, 0, width // 2, height), left_dir / f"L_{i}.{OUT_EXT}", scale_factor)
                    self._write_cropped_png(img, (width // 2, 0, width, height), right_dir / f"R_{i}.{OUT_EXT}", scale_factor)
                    converted_count += 1
            except Exception as e:
                logger.error(f"Failed to process {source_dds}: {e}")

        # --- 3. Update the hash in the cache ---
        self.config_manager.update_cached_paths(
            cached_paths.briefing_file_path or "", # Preserve the old briefing path
            current_hash
        )

        return {"success": True, "message": f"Converted {converted_count} DDS files.", "cached": False}