# Architecture Overview

This document describes the high-level architecture of the BMS Bridge project.

## Guiding Principles

The project's architecture is built on two core principles:

1.  **Use the Right Tool for the Job:** We leverage the distinct strengths of both the C#/.NET and Python ecosystems. C# is ideal for creating a native Windows GUI application, while Python excels at rapid web backend development and data processing.
2.  **Decoupling and Resilience:** The Launcher (management client) and the Server (backend) are two separate, independent applications. This separation ensures that a failure in one component does not bring down the entire system.

## Core Components: The "Launcher-Server" Model

### 1. C# WinForms Launcher (`BMS_Bridge_Launcher`)

A lightweight native Windows application that serves as the user's control center.

**Responsibilities:**
*   **Process Management:** Starting and reliably stopping the Python server process.
*   **UI Feedback:** Displaying the real-time status of both the server and its connection to the simulator.
*   **System Integration:** Providing a familiar user experience through a system tray icon.
*   **Configuration Hub:** The future entry point for all user-facing settings.

**Key Implementation Details:**
*   The code is structured according to the **Single Responsibility Principle**. The `Form1` class acts as a coordinator, delegating tasks to specialized classes: `ServerManager` (process control), `HealthMonitor` (API polling), and `UIController` (UI updates).
*   It interacts with the server as a "black box" through three primary mechanisms:
    1.  **Health API Polling:** A `HealthMonitor` class periodically queries the server's `/api/health` endpoint. This is the **single source of truth** for the UI.
    2.  **PID File Management:** A `ServerManager` class launches the server as a fully independent process, saves its Process ID (PID) to a `server.pid` file, and uses this PID for precise and reliable termination.
    3.  **Log File Monitoring:** The `ServerManager` uses a `FileSystemWatcher` to monitor the server's log file in real-time and display its contents in the UI, removing the need for direct process stream redirection.

### 2. Python FastAPI Server (`Server_Core`)

The engine of the project. A console application compiled into a standalone `.exe` that handles all data processing.

**Responsibilities:**
*   **Data Acquisition:** Connecting to Falcon BMS Shared Memory.
*   **File Processing:** Parsing and caching briefing files.
*   **Image Conversion:** Converting and caching kneeboard charts.
*   **Web Server:** Serving a modern Single-Page Application (SPA) frontend.
*   **API Provider:** Exposing a REST API (`/api/health`) and a WebSocket (`/ws/flight_data`).

**Key Implementation Details:**
*   **FastAPI:** Chosen for its high performance and async capabilities.
*   **Autonomous:** Bundled into a single `.exe` with PyInstaller.
*   **Standalone:** Designed as a full-featured console application that can be configured and run without the GUI launcher.

## Interaction Flow

1.  The **C# Launcher** starts the **Python Server** as an independent process and saves its PID to `server.pid`.
2.  The **Launcher's UI** begins polling the Server's `/api/health` endpoint.
3.  The **Server** responds with its status and its connection status to the simulator.
4.  The user opens a **Web Browser** on a phone/tablet and navigates to the Server's IP address.
5.  The **Server** serves the `index.html` and `app.js` files.
6.  The **JavaScript App** in the browser connects to the Server's WebSocket for real-time data.
7.  When finished, the user uses the **C# Launcher**, which reads the PID from `server.pid` and terminates the specific **Python Server** process.