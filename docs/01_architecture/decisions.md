# Architectural Decision Record (ADR)

This document records the key architectural decisions made during the project's development, along with their context and consequences.

---

### ADR-001: Choice of Hybrid "Launcher-Server" Architecture vs. Monolith

**Context:**
The initial architectural choice was to decide between three possible approaches:
1.  **Hybrid (Current):** A C# WinForms GUI launcher controlling a separate Python/FastAPI web server.
2.  **Python Monolith:** A single Python application using a GUI framework like PyQt/PySide for both the backend logic and the user interface.
3.  **C# Monolith:** A single C# application handling all logic, including the complex task of reading data from the simulator's Shared Memory.

**Decision:**
We have explicitly chosen the **Hybrid "Launcher-Server" architecture.**

**Rationale:**
*   **Best Tool for the Job:** This approach allows us to leverage the specific strengths of each technology stack. C# and WinForms are unparalleled for creating a native, lightweight, and deeply integrated Windows UI (especially for system tray functionality). Python, with its FastAPI framework and rich library ecosystem (`psutil`, `Pillow`), is the superior choice for rapid backend and data processing development.
*   **Decoupling & Resilience:** The separation of the control plane (Launcher) from the data plane (Server) is a significant advantage. A crash in the Python server (e.g., due to a malformed briefing file) will not crash the C# launcher, which can then be used to restart the server. This isolation increases the overall stability of the system.
*   **Preservation of the Core "Second Screen" Feature:** A key requirement is the ability to access the interface from any device (phones, tablets). A web-based backend is the only feasible way to achieve this. A monolithic GUI application (either Python or C#) would lose this critical feature or require running a web server in a separate thread, which would re-introduce the complexity we sought to avoid.
*   **Avoidance of Extreme Complexity:** The C# Monolith approach was rejected due to the prohibitive difficulty of porting the C++ Shared Memory structures to C#. This task would require complex, low-level P/Invoke calls and manual memory layout management, making the project extremely difficult to maintain. The Python Monolith approach was rejected due to the significant increase in distribution size and complexity caused by packaging GUI libraries like PyQt with PyInstaller.

**Consequences:**
The primary drawback of this approach is the complexity of inter-process communication (IPC). Significant development effort has been invested in creating a reliable start/stop mechanism and an API-based status monitoring system. This complexity is considered a worthwhile trade-off for the architectural flexibility and resilience gained.

---

### ADR-002: Server Launch Method - Independent Process vs. Child Process

**Context:**
The C# launcher needs to execute the Python server. This can be done by launching it as a direct child process or as a completely independent process.

**Decision:**
The server is to be launched as a **fully independent process.**

**Rationale:**
*   **Increased Robustness:** The primary goal is to ensure the server's uptime is not tied to the launcher's. If the launcher crashes or is closed, the server should continue running without interruption, as it is the core component providing the service to the user's web client.
*   **Clear Separation of Roles:** This decision reinforces the architectural principle of decoupling. The launcher's role is that of a "remote control," not a "parent" or "host." The server acts as a standalone service.
*   **Mitigated Downsides:** The main disadvantage of this approach is the loss of direct process control and output stream redirection. However, these issues have been addressed:
    *   **Process Control:** Server termination is handled reliably by the `taskkill /T /IM <name>` command, which finds and terminates the process by its name, regardless of parent-child relationships.
    *   **Logging:** Real-time log display in the launcher is achieved by monitoring the server's log file on disk using a `FileSystemWatcher`, completely bypassing the need for stream redirection.

**Consequences:**
The C# launcher cannot assume it "owns" or has exclusive control over the server process. This led to the implementation of the "server-aware startup" feature, where the launcher "pings" the health API on startup to detect and "adopt" an already-running server instance.