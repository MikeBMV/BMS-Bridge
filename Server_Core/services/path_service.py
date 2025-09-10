import winreg
import platform
from pathlib import Path
from typing import Optional
import logging

from adapters.bms_adapter import BMSAdapter

logger = logging.getLogger(__name__)

class PathService:
    BMS_VERSION = "4.37"
    WIN_REG_KEY = rf"SOFTWARE\WOW6432Node\Benchmark Sims\Falcon BMS {BMS_VERSION}"
    BRIEFINGS_SUBPATH = "User/Briefings"

    def __init__(self):
        self._bms_base_dir_from_registry: Optional[Path] = self._find_bms_base_dir_from_registry()

    def _find_bms_base_dir_from_registry(self) -> Optional[Path]:
        if platform.system() != "Windows":
            return None
        try:
            with winreg.ConnectRegistry(None, winreg.HKEY_LOCAL_MACHINE) as hkey:
                with winreg.OpenKey(hkey, self.WIN_REG_KEY) as subkey:
                    base_dir_str, _ = winreg.QueryValueEx(subkey, "baseDir")
            
            if base_dir_str and Path(base_dir_str).is_dir():
                logger.info(f"Found BMS base directory in registry: {base_dir_str}")
                return Path(base_dir_str)
        except OSError:
            logger.warning("Could not find BMS installation in Windows registry.")
        except Exception:
            logger.error("An unexpected error occurred while reading Windows registry.", exc_info=True)
        
        return None

    def find_briefings_dir(self, bms_adapter: BMSAdapter) -> Optional[Path]:
        flight_data = bms_adapter.get_all_data()
        if flight_data and (live_dir_str := flight_data.get("BmsBriefingsDirectory")):
            live_dir = Path(live_dir_str.strip())
            if live_dir.is_dir():
                logger.info(f"Found live briefings directory from Shared Memory: {live_dir}")
                return live_dir

        if self._bms_base_dir_from_registry:
            offline_dir = self._bms_base_dir_from_registry / self.BRIEFINGS_SUBPATH
            if offline_dir.is_dir():
                logger.info(f"Using cached briefings directory from registry: {offline_dir}")
                return offline_dir
        
        logger.error("Could not find briefing directory via Shared Memory or Registry.")
        return None