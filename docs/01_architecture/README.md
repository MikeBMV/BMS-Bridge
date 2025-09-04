# Architecture Overview

This document describes the high-level architecture of the BMS Bridge project.

## Guiding Principles

The project's architecture is built on two core principles:

1.  **Use the Right Tool for the Job:** We leverage the distinct strengths of both the C#/.NET and Python ecosystems. C# is ideal for creating a native Windows GUI application for management, while Python excels at rapid web backend development and data processing.
2.  **Decoupling and Resilience:** The Launcher (management client) and the Server (backend) are two separate, independent applications. This separation ensures that a failure in one component does not bring down the entire system.

## Core Components: The "Launcher-Server" Model

### 1. C# WinForms Launcher (`BMS_Bridge_Launcher`)

A native Windows application that serves as the user's control and configuration center.

**Responsibilities:**
*   **Process Management:** Starting and reliably stopping the Python server process using a PID file.
*   **UI Feedback:** Displaying the real-time status of the server and its connection to the simulator.
*   **System Integration:** Providing a familiar user experience through a system tray icon.
*   **Configuration Hub:** **(New)** Providing a user-friendly graphical interface for managing complex server settings, such as the Kneeboard file lists. The launcher is the **master** for this configuration.

**Key Implementation Details:**
*   The code is structured according to the **Single Responsibility Principle**. The `Form1` class acts as a coordinator, delegating tasks to specialized classes: `ServerManager` (process control), `HealthMonitor` (API polling), and `UIController` (UI updates).
*   **Configuration Management:** The launcher reads and writes directly to the server's central configuration file: `Server/config/settings.json`. This makes the launcher the primary tool for configuring features like the Kneeboards.

### 2. Python FastAPI Server (`Server_Core`)

The engine of the project. A console application compiled into a standalone `.exe` that handles all data processing and serves the web interface.

**Responsibilities:**
*   **Data Acquisition:** Connecting to Falcon BMS Shared Memory.
*   **Web Server:** Serving a modern Single-Page Application (SPA) frontend to any device on the local network.
*   **API Provider:** Exposing a REST API (`/api/health`, `/api/kneeboards/*`) and a WebSocket (`/ws/flight_data`).
*   **Configuration Consumer:** **(New)** The server is a **consumer** of the `Server/config/settings.json` file. It reads this configuration on startup and on-demand to understand what content to serve (e.g., which kneeboard files to display, in what order).

**Key Implementation Details:**
*   **FastAPI:** Chosen for its high performance and async capabilities.
*   **Autonomous:** Bundled into a single `.exe` with PyInstaller. It can be configured and run manually without the GUI launcher, making it flexible for advanced users.
*   **User Content Storage:** **(New)** All user-provided files (images, PDFs for kneeboards) are copied by the launcher into a dedicated, safe directory: `Server/user_data/kneeboards/`. The server serves files only from this location, ensuring stability and security.

## Interaction Flow (Updated)

1.  **(Configuration)** The user interacts with the **C# Launcher** to add, remove, and reorder kneeboard pages.
2.  The **Launcher** saves this configuration directly into `Server/config/settings.json` and copies the user's files into `Server/user_data/kneeboards/`.
3.  **(Process Start)** The user clicks "Start Server". The **C# Launcher** starts the **Python Server** as an independent process, passing a `--hide-console` flag. It saves the server's PID to `server.pid`.
4.  The **Launcher's UI** begins polling the Server's `/api/health` endpoint to monitor its status.
5.  **(Web Client)** The user opens a **Web Browser** on a phone/tablet and navigates to the Server's IP address.
6.  The **Server** serves the main `index.html` and `app.js` files.
7.  The **JavaScript App** in the browser makes a request to the server's `/api/kneeboards/left` endpoint.
8.  The **Server** reads `settings.json`, builds a list of enabled kneeboard pages in the correct order, and sends this list back to the browser.
9.  The **JavaScript App** then requests each specific image/PDF file (e.g., `/user_data/kneeboards/MyMap.png`), which the server serves from the safe storage directory.
10. **(Process Stop)** When finished, the user uses the **C# Launcher**, which reads the PID from `server.pid` and terminates the specific **Python Server** process.