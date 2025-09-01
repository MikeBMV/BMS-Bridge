# Current Roadmap: Core Functionality Enhancements

This document outlines the high-priority tasks for the immediate development cycle. The focus of this cycle is to improve the fundamental usability and robustness of the launcher application before adding major new features.

## Task 1: Implement Single-Instance Application Logic

**Status:** Planned
**Priority:** High

**Problem:**
Currently, the user can launch multiple instances of the `BMS_Bridge_Launcher.exe`. This is undesirable for a tray-based application as it leads to user confusion, multiple tray icons, and potential conflicts in controlling the server process.

**Requirements:**
*   When a user attempts to launch a second instance of the application while one is already running, the new instance must not start.
*   Instead, the existing, primary instance should be activated.
*   "Activation" means:
    *   If the main window of the primary instance is minimized or in the background, it should be brought to the foreground and activated.
    *   If the primary instance is currently hidden in the system tray, its main window should be restored and brought to the foreground.
*   The second process instance must exit cleanly and immediately after signaling the primary instance.

**Implementation Notes:**
This will be implemented in the C# launcher using a named `Mutex` to detect existing instances. Inter-process communication to activate the first instance will likely be handled via WinAPI calls (`FindWindow`, `SetForegroundWindow`, `ShowWindow`).

---

## Task 2: Implement "Live Server" Detection on Startup

**Status:** Planned
**Priority:** High

**Problem:**
Due to the decision to run the server as an independent process (see `ADR-002`), the launcher may be closed or crash while the server continues to run. When the launcher is restarted, it currently has no knowledge of the existing server process and incorrectly displays a "Stopped" status.

**Requirements:**
*   On application startup, before the UI becomes fully interactive, the launcher must perform a "health check" against the default server endpoint (`http://localhost:8000/api/health`).
*   **If the health check succeeds:** The launcher must "adopt" the existing server. This means it should immediately transition to a monitoring state:
    *   The status indicators must reflect the state received from the API.
    *   The "Start/Stop" button must display "Stop Server".
    *   The polling timer for the health API must be started.
*   **If the health check fails:** The launcher should start in its normal, "cold" state, displaying a "Stopped" status.

**Implementation Notes:**
This logic will be added to the `InitializeApp` method in `Form1.cs`. The initial health check will be performed there, and the UI will be updated accordingly.

---

## Task 3: Externalize All Hardcoded Configuration

**Status:** Planned
**Priority:** Critical (Blocker for new features)

**Problem:**
As identified in the expert review, numerous critical parameters are hardcoded throughout the codebase (BMS version, DDS file numbers, server port, etc.). This makes the application extremely brittle and difficult to maintain or adapt.

**Requirements:**
*   **Python Server:** All "magic values" (BMS version, registry keys, DDS paths, DDS numbers) currently in `.py` files must be moved into `Server_Core/config/settings.json`. The Python code must be refactored to read these values via the `ConfigManager`.
*   **C# Launcher:** The server endpoint URL (`http://localhost:8000`) must be externalized. A simple local `config.json` or using application settings (`app.config`) is acceptable.
*   The `settings.json` file must become the single source of truth for all configurable parameters.

**Implementation Notes:**
This task requires careful refactoring across multiple files. It must be completed **before** work begins on the Kneeboard Management feature, as that feature will also depend on this configuration system.