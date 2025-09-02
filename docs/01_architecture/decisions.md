# Architectural Decision Record (ADR)

This document records the key architectural decisions made during the project's development.

---

### ADR-001: Choice of Hybrid "Launcher-Server" Architecture vs. Monolith

**Context:**
Three approaches were considered:
1.  **Hybrid (Current):** C# GUI Launcher + Python/FastAPI Server.
2.  **Python Monolith:** A single Python app with a PyQt/PySide GUI.
3.  **C# Monolith:** A single C# app handling all logic.

**Decision:**
The **Hybrid Architecture** was definitively chosen.

**Rationale:**
*   **Best Tool for the Job:** C# is ideal for native Windows UI. Python is superior for the backend and data processing.
*   **Resilience:** Component separation increases stability. A server crash does not take down the launcher.
*   **Preservation of "Second Screen":** Only a web server backend allows access from any device (phones/tablets).
*   **Avoidance of Extreme Complexity:** A C# monolith would face prohibitive difficulty in porting C++ Shared Memory structures. A Python monolith would result in a massive distribution size and GUI library bundling complexities.

---

### ADR-002: Server Launch Method - Independent Process vs. Child Process

**Context:**
The launcher can run the server as a tightly coupled child process or as a decoupled, independent one.

**Decision:**
The server is launched as a **fully independent process**.

**Rationale:**
*   **Robustness:** The server's uptime must not be tied to the launcher's. If the launcher crashes, the server must continue running to serve the web client.
*   **Clear Separation of Roles:** This reinforces the architectural principle of decoupling. The launcher is a "remote control," not a "host."
*   **Mitigated Downsides:**
    *   **Control:** The problem of controlling an independent process is solved by using a PID file.
    *   **Monitoring:** The problem of viewing logs is solved by reading the log file directly from disk.

---

### ADR-003: Server Termination Method

**Context:**
Several server shutdown methods were tested during development.
1.  `Process.Kill()` on a child process.
2.  `taskkill /IM` (kill by image name).
3.  `taskkill /T /IM` (kill process tree by image name).

**Decision:**
To use **targeted process termination via its Process ID (PID)**, which is stored in a file.

**Rationale:**
*   **Problem with `Kill()` and PyInstaller:** It was discovered that PyInstaller creates a process tree. `Process.Kill()` only terminated the parent bootstrapper, leaving the actual server running.
*   **Problem with `taskkill /IM`:** Expert reviews correctly identified this method as "brittle," as it could accidentally terminate an unrelated process with the same name.
*   **Final Solution:** The launcher saves the server's PID to `server.pid` upon launch. To stop, it reads the PID from this file and terminates that **specific** process. This is the most precise, reliable, and professional method.