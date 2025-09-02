# Current Roadmap: Core Enhancements

This document outlines the high-priority tasks for the immediate development cycle. The focus is to improve the fundamental stability and robustness of the application before adding major new features.

### Task 1: Refactor Launcher - Implement `ServerManager`

**Status:** Planned
**Priority:** High

**Problem:**
As noted by expert reviews, the `Form1.cs` class is overloaded with responsibilities (UI, process management, API client), violating the Single Responsibility Principle (SRP).

**Requirements:**
*   Create a new `ServerManager.cs` class.
*   Move all non-UI logic into it:
    *   Starting the server as an independent process.
    *   Logic for saving and reading the PID file (`server.pid`).
    *   Logic for stopping the process by PID.
    *   Logic for monitoring the log file via `FileSystemWatcher`.
*   `Form1.cs` should only contain UI logic and call simple methods like `serverManager.Start()` and `serverManager.Stop()`.

---

### Task 2: Implement Single-Instance Application

**Status:** Planned
**Priority:** High

**Problem:**
The user can launch multiple instances of the launcher, leading to conflicts.

**Requirements:**
*   A second instance should activate the window of the first instance (even from the tray) and then immediately exit.

---

### Task 3: Implement "Live Server" Adoption on Startup

**Status:** Planned
**Priority:** High

**Problem:**
If the launcher crashes while the server remains running, a restarted launcher is unaware of it.

**Requirements:**
*   On startup, the launcher must first check the server's health API. If the server is already running, the launcher must "adopt" it (begin monitoring) instead of trying to start a new process.

---

### Task 4: Externalize All Hardcoded Configuration

**Status:** Planned
**Priority:** Critical (Blocker for new features)

**Problem:**
Key parameters (BMS version, DDS numbers, ports) are scattered throughout the code.

**Requirements:**
*   **Python Server:** Move all "magic numbers" to `Server_Core/config/settings.json`.
*   **Python Server:** Implement the ability to override settings from `settings.json` via command-line arguments (`argparse`), allowing the server to function as a true standalone console app.
*   **C# Launcher:** Externalize the server URL to a configuration file.