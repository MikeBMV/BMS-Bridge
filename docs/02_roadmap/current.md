# Current Roadmap: Core Enhancements

This document outlines the high-priority tasks for the immediate development cycle. The focus is to improve the fundamental stability and robustness of the application before adding major new features.

### Task 1: Refactor Launcher - Implement SRP

**Status:** Done

**Problem:**
As noted by expert reviews, the `Form1.cs` class was overloaded with responsibilities, violating the Single Responsibility Principle (SRP).

**Result:**
The C# launcher was successfully refactored into a multi-class architecture:
*   `ServerManager`: Handles the server process lifecycle.
*   `HealthMonitor`: Manages all `/api/health` polling.
*   `UIController`: Centralizes all UI update logic.
*   `Form1`: Acts as a coordinator, delegating tasks. This greatly improved code readability and maintainability.

---

### Task 2: Implement Single-Instance Application

**Status:** Done

**Problem:**
The user could launch multiple instances of the launcher, leading to conflicts.

**Result:**
The application now uses a system-wide Mutex to ensure only one instance can run. Attempting to launch a second instance will instead bring the original instance's window to the foreground, even if it was minimized to the system tray (using a `WM_SHOWME` message).

---

### Task 3: Implement "Live Server" Adoption on Startup

**Status:** Planned
**Priority:** High

**Problem:**
If the launcher crashes while the server remains running, a restarted launcher is unaware of it.

**Requirements:**
*   On startup, the launcher must first check for a running server (via PID in `settings.json` and/or an API health check). If a server is found, the launcher must "adopt" it (begin monitoring) instead of trying to start a new process.

---

### Task 4: Externalize All Hardcoded Configuration

**Status:** In Progress
**Priority:** Critical

**Problem:**
Key parameters were scattered throughout the code.

**Requirements:**
*   **Python Server:** Move all "magic numbers" to `Server_Core/config/settings.json`. **(Done)**
*   **Python Server:** Implement the ability to override settings via command-line arguments. **(Partially Done - `--hide-console` is implemented)**
*   **C# Launcher:** Externalize the server URL to a configuration file. **(To Do)**