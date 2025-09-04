# File: services/briefing_service.py - CORRECTED
import re
import time
import logging
from pathlib import Path
from typing import Dict, Any, List, Optional

from config.settings import ConfigManager
from adapters.bms_adapter import BMSAdapter

logger = logging.getLogger(__name__)

class BriefingCache:
    def __init__(self, ttl_seconds: int): self.ttl_seconds = ttl_seconds; self._cache: Dict[str, Any] = {}; self._cache_time: float = 0
    def get(self) -> Optional[Dict[str, Any]]:
        if time.time() - self._cache_time < self.ttl_seconds: return self._cache
        return None
    def set(self, data: Dict[str, Any]): self._cache = data; self._cache_time = time.time()

class BriefingParser:
    SECTION_HEADERS = ["Mission Overview:", "Pilot Roster:", "Package Elements:", "Threat Analysis:", "Steerpoints:", "Comm Ladder:", "Iff", "Link 16", "Ordnance:", "Weather:", "Support:", "Emergency Procedures:"]
    @staticmethod
    def _parse_tab_separated_table(content: List[str]) -> Optional[Dict[str, Any]]:
        header_line, header_line_index = "", -1
        for i, line in enumerate(content):
            if '\t' in line and ':' in line: header_line, header_line_index = line, i; break
        if not header_line: return None
        headers = [h.strip().replace(':', '') for h in re.split(r'\t+', header_line.strip()) if h.strip()]
        if not headers: return None
        rows = [dict(zip(headers, [p.strip() for p in re.split(r'\t+', line.strip())])) for line in content[header_line_index + 1:] if line.strip() and not line.strip().startswith('---')]
        return {"headers": headers, "rows": [r for r in rows if any(r.values())]} if rows else None
    @staticmethod
    def _parse_key_value(content: List[str]) -> Dict[str, Any]:
        data, full_text_lines = {}, []
        for line in content:
            clean_line = line.strip()
            if ":" in clean_line:
                parts = clean_line.split(":", 1)
                if len(parts) == 2 and parts[0].strip(): data[parts[0].strip()] = parts[1].strip(); continue
            if clean_line: full_text_lines.append(clean_line)
        if data: return {"type": "kv_list", "data": data}
        return {"type": "text", "data": "\n".join(full_text_lines)}
    @staticmethod
    def _parse_complex_section(content: List[str]) -> Dict[str, Any]:
        sub_sections, current_sub = [], None
        for line in content:
            stripped = line.strip()
            if not stripped: continue
            if stripped.endswith(':') and len(stripped.split()) <= 3:
                if current_sub: sub_sections.append(current_sub)
                current_sub = {"title": stripped, "content": []}
            elif current_sub: current_sub["content"].append(line)
            else: current_sub = {"title": "General", "content": [line]}
        if current_sub: sub_sections.append(current_sub)
        processed_subs = []
        for sub in sub_sections:
            table_data = BriefingParser._parse_tab_separated_table(sub["content"])
            if table_data: processed_subs.append({"title": sub["title"], "type": "table", "data": table_data})
            else: processed_subs.append({"title": sub["title"], **BriefingParser._parse_key_value(sub["content"])})
        return {"type": "complex_section", "data": processed_subs}
    @staticmethod
    def parse_briefing(text: str) -> Dict[str, Any]:
        all_sections, current_section_name, section_content = {}, "Header", []
        for line in text.splitlines():
            stripped_line = line.strip()
            found_header = next((h.replace(":", "").strip() for h in BriefingParser.SECTION_HEADERS if stripped_line.lower().startswith(h.lower().replace(":", ""))), None)
            if found_header:
                if section_content and current_section_name != "Header": all_sections[current_section_name] = section_content
                current_section_name = found_header
                content_after_header = line[len(found_header):].lstrip(': ').strip()
                section_content = [content_after_header] if content_after_header else []
            else: section_content.append(line)
        if section_content: all_sections[current_section_name] = section_content
        processed_sections = {}
        for name, content in all_sections.items():
            content = [l for l in content if l.strip()]
            if not content: continue
            result_data = None
            if name in ["Steerpoints", "Comm Ladder", "Pilot Roster", "Package Elements", "Weather", "Ordnance"]:
                table_data = BriefingParser._parse_tab_separated_table(content)
                if table_data: result_data = {"type": "table", "className": name.lower().replace(" ", "-") + "-table", "data": table_data}
            elif name in ["Iff", "Link 16"]: result_data = BriefingParser._parse_complex_section(content)
            processed_sections[name] = result_data if result_data else BriefingParser._parse_key_value(content)
        page_definitions = {"Overview": ["Mission Overview", "Pilot Roster", "Package Elements", "Threat Analysis", "Ordnance", "Support", "Emergency Procedures"], "Steerpoints": ["Steerpoints"], "Comm Ladder": ["Comm Ladder"], "IFF": ["Iff"], "Link 16": ["Link 16"], "Weather": ["Weather"]}
        pages = [{"title": page_title, "sections": [{"name": key, **processed_sections[key]} for key in section_keys if key in processed_sections]} for page_title, section_keys in page_definitions.items()]
        return {"pages": [p for p in pages if p["sections"]]}

class BriefingService:
    def __init__(self, config_manager: ConfigManager):
        self.config_manager = config_manager
        self.parser = BriefingParser()
        self.cache = BriefingCache(config_manager.load_config().file_cache_ttl_seconds)
        self._last_file_path: Optional[str] = None
        self._last_file_mtime: float = 0.0

    def _find_briefing_file(self, bms_adapter: BMSAdapter) -> Optional[str]:
        briefing_file_path = None
        try:
            if bms_adapter.is_connected():
                all_data = bms_adapter.get_all_data()
                if all_data and (live_dir := all_data.get("BmsBriefingsDirectory")):
                    briefing_path = Path(live_dir.strip()) / "briefing.txt"
                    if briefing_path.is_file():
                        briefing_file_path = str(briefing_path)
        except Exception: pass

        if not briefing_file_path:
            # cached = self.config_manager.get_cached_paths().briefing_file_path
            # if cached and Path(cached).is_file(): briefing_file_path = cached
            pass
        
        return briefing_file_path

    def get_briefing_data(self, bms_adapter: BMSAdapter) -> Dict[str, Any]:
        briefing_file_path = self._find_briefing_file(bms_adapter)
        if not briefing_file_path:
            return {"success": False, "error": "Briefing file not found"}

        try:
            current_mtime = Path(briefing_file_path).stat().st_mtime
            if briefing_file_path == self._last_file_path and current_mtime == self._last_file_mtime:
                cached_data = self.cache.get()
                if cached_data:
                    logger.info("Returning cached briefing data.")
                    return {"success": True, "data": cached_data, "cached": True}

            logger.info(f"Parsing new or updated briefing file: {briefing_file_path}")
            content = Path(briefing_file_path).read_text(encoding='utf-8', errors='ignore')
            parsed_data = self.parser.parse_briefing(content)
            self.cache.set(parsed_data)
            self._last_file_path = briefing_file_path
            self._last_file_mtime = current_mtime
            
            # self.config_manager.update_cached_paths(briefing_file_path, None)

            return {"success": True, "data": parsed_data, "cached": False}
        except Exception as e:
            logger.error(f"Failed to process briefing file: {e}")
            return {"success": False, "error": str(e)}