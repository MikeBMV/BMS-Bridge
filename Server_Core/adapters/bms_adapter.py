# File: adapters/bms_adapter.py
import ctypes
import mmap
import platform
import struct
import threading
import time
from typing import Dict, Any, Optional
from enum import Enum
import logging

import psutil
from falcon_memreader import FlightData, FlightData2, StringData

logger = logging.getLogger(__name__)

# --- 1. Classes for the Circuit Breaker pattern implementation ---

class CircuitBreakerState(Enum):
    CLOSED = "CLOSED"      # The connection is working
    OPEN = "OPEN"          # The connection is broken, no attempts are made
    HALF_OPEN = "HALF_OPEN"  # A single attempt is made to restore the connection

class CircuitBreaker:
    """Protects the system from failures by preventing repeated calls to a failing service."""
    def __init__(self, failure_threshold: int, reset_timeout: int):
        self.failure_threshold = failure_threshold
        self.reset_timeout = reset_timeout
        self.failure_count = 0
        self.state = CircuitBreakerState.CLOSED
        self.last_failure_time: Optional[float] = None
        self._lock = threading.Lock()

    def call(self, func, *args, **kwargs):
        """Executes a function protected by the Circuit Breaker."""
        with self._lock:
            if self.state == CircuitBreakerState.OPEN:
                if time.time() - (self.last_failure_time or 0) > self.reset_timeout:
                    self.state = CircuitBreakerState.HALF_OPEN
                    logger.info("Circuit breaker is now HALF_OPEN. Attempting one call.")
                else:
                    raise ConnectionError("Circuit breaker is OPEN. Call is blocked.")
        
        try:
            result = func(*args, **kwargs)
            self._on_success()
            return result
        except Exception as e:
            self._on_failure()
            # Re-raise the exception so the calling code knows about the problem
            raise e

    def _on_success(self):
        with self._lock:
            if self.state == CircuitBreakerState.HALF_OPEN:
                logger.info("Circuit breaker is now CLOSED after successful call.")
            self.state = CircuitBreakerState.CLOSED
            self.failure_count = 0
            self.last_failure_time = None

    def _on_failure(self):
        with self._lock:
            self.failure_count += 1
            self.last_failure_time = time.time()
            if self.failure_count >= self.failure_threshold:
                self.state = CircuitBreakerState.OPEN
                logger.warning(f"Circuit breaker is now OPEN after {self.failure_count} failures.")

# --- 2. Class for data conversion (remains unchanged) ---

class BMSDataConverter:
    # This class remains the same as in our last working version
    @staticmethod
    def convert_value(value: Any) -> Any:
        if isinstance(value, ctypes.Array):
            if value._type_ == ctypes.c_char:
                try: return value.value.decode('utf-8', errors='ignore').strip('\x00')
                except: return ""
            else: return [BMSDataConverter.convert_value(item) for item in value]
        elif isinstance(value, bytes):
            try: return value.decode('utf-8', errors='ignore').strip('\x00')
            except: return ""
        return value

    @staticmethod
    def convert_struct_to_dict(struct: Any, max_array_size: int = 100) -> Dict[str, Any]:
        result = {}
        for field_name, _ in struct._fields_:
            try:
                value = getattr(struct, field_name)
                if isinstance(value, ctypes.Array) and len(value) > max_array_size: continue
                result[field_name] = BMSDataConverter.convert_value(value)
            except Exception as e:
                logger.debug(f"Failed to convert field {field_name}: {e}")
                continue
        return result

# --- 3. Main adapter class with the Circuit Breaker ---

class BMSAdapter:
    BMS_EXECUTABLE = "Falcon BMS.exe"

    def __init__(self, failure_threshold: int, reset_timeout: int):
        self.flight_data_area: Optional[mmap.mmap] = None
        self.flight_data_2_area: Optional[mmap.mmap] = None
        self.string_data_area: Optional[mmap.mmap] = None
        self._is_connected = False
        self.converter = BMSDataConverter()
        self.circuit_breaker = CircuitBreaker(failure_threshold, reset_timeout)
        self._process_check_cache = {'running': False, 'time': 0}

    def is_bms_process_running(self) -> bool:
        """Checks if the BMS process is running, with a 5-second cache for the result."""
        now = time.time()
        if now - self._process_check_cache['time'] < 5.0:
            return self._process_check_cache['running']

        if platform.system() != "Windows":
            self._process_check_cache = {'running': False, 'time': now}
            return False
            
        is_running = any(proc.info['name'] == self.BMS_EXECUTABLE for proc in psutil.process_iter(['name']))
        self._process_check_cache = {'running': is_running, 'time': now}
        return is_running

    def _connect_internal(self):
        """Internal method that performs the actual connection."""
        if not self.is_bms_process_running():
            raise ConnectionError("Falcon BMS process is not running.")
        
        try:
            self.flight_data_area = mmap.mmap(-1, ctypes.sizeof(FlightData), FlightData.name, access=mmap.ACCESS_READ)
            self.flight_data_2_area = mmap.mmap(-1, ctypes.sizeof(FlightData2), FlightData2.name, access=mmap.ACCESS_READ)
            self.string_data_area = mmap.mmap(-1, StringData.area_size_max, StringData.name, access=mmap.ACCESS_READ)
            self._is_connected = True
            logger.info("Successfully connected to BMS Shared Memory.")
        except FileNotFoundError:
            self._is_connected = False
            raise ConnectionError("BMS Shared Memory not found (is the simulator in 3D?)")

    def connect(self):
        """Public method for connecting, protected by the Circuit Breaker."""
        if self._is_connected: return
        self.circuit_breaker.call(self._connect_internal)

    def is_connected(self) -> bool:
        return self._is_connected

    def close(self):
        for area in [self.flight_data_area, self.flight_data_2_area, self.string_data_area]:
            if area and not area.closed: area.close()
        self._is_connected = False
        logger.info("BMS Shared Memory connection closed.")


    def _read_string_data(self) -> Optional[Dict[str, str]]:
        if not self.string_data_area or self.string_data_area.closed:
            return None

        try:
            self.string_data_area.seek(0)
            
            version_num = struct.unpack('I', self.string_data_area.read(4))[0]
            num_strings = struct.unpack('I', self.string_data_area.read(4))[0]
            data_size = struct.unpack('I', self.string_data_area.read(4))[0]
            
            strings = {}
            for key in StringData.id:
                str_id = struct.unpack('I', self.string_data_area.read(4))[0]
                str_length = struct.unpack('I', self.string_data_area.read(4))[0]
                str_data = self.string_data_area.read(str_length + 1).decode('utf-8', errors='ignore').rstrip('\x00')
                strings[key] = str_data
                
            return strings
        except struct.error:
            logger.warning("Failed to unpack StringData, memory layout might have changed.")
            self.close()
            return None

    def _get_all_data_internal(self) -> Dict[str, Any]:
        """Internal method that reads the data."""
        if not self._is_connected:
            raise ConnectionError("Not connected to BMS Shared Memory.")
        
        try:
            self.flight_data_area.seek(0)
            self.flight_data_2_area.seek(0)
            flight_data_struct = FlightData.from_buffer_copy(self.flight_data_area.read(ctypes.sizeof(FlightData)))
            flight_data_2_struct = FlightData2.from_buffer_copy(self.flight_data_2_area.read(ctypes.sizeof(FlightData2)))
            
            dict1 = self.converter.convert_struct_to_dict(flight_data_struct)
            dict2 = self.converter.convert_struct_to_dict(flight_data_2_struct)
            dict3 = self._read_string_data()
            if dict3 is None:
                logger.debug("StringData shared memory area not available or failed to read.")
                dict3 = {}
            return {**dict1, **dict2, **dict3} 
        except Exception as e:
            logger.warning(f"Failed to read from Shared Memory, closing connection: {e}")
            self.close()
            raise ConnectionError(f"Failed to read BMS data: {e}")

    def get_all_data(self) -> Optional[Dict[str, Any]]:
        """Public method for getting data, protected by the Circuit Breaker."""
        if not self._is_connected:
            try:
                self.connect()
            except ConnectionError as e:
                logger.debug(f"Connection attempt failed: {e}")
                return None

        try:
            return self.circuit_breaker.call(self._get_all_data_internal)
        except ConnectionError as e:
            logger.debug(f"Data read failed: {e}")
            return None