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
*   **Process Management:** Starting and reliably stopping the Python server process using a Process ID (PID).
*   **UI Feedback:** **(Updated)** Displaying the real-time status of the server and its connection to the simulator in a compact `ToolStrip` header.
*   **System Integration:** Providing a familiar user experience through a system tray icon and ensuring only a single instance of the application can run at a time (via Mutex).
*   **Configuration Hub:** Providing a user-friendly graphical interface for managing complex server settings, such as the Kneeboard file lists. The launcher is the **master** for this configuration.

**Key Implementation Details:**
*   **Single Responsibility Principle:** The code is structured cleanly. The `Form1` class acts as a coordinator, delegating tasks to specialized classes: `ServerManager` (process control), `HealthMonitor` (API polling), and `UIController` (UI updates).
*   **Configuration Management:** The launcher reads and writes directly to the server's central configuration file: `Server/config/settings.json`.
*   **(New) Modernized UI:** The main control panel has been refactored from a bulky header into a clean `ToolStrip` docked to the top of the window. This saves vertical space and consolidates all status information and controls (Start/Stop, Settings, QR Code) into a single bar.

### 2. Python FastAPI Server (`Server_Core`)

The engine of the project. A console application compiled into a standalone `.exe` that handles all data processing and serves the web interface.

**Responsibilities:**
*   **Data Acquisition:** Connecting to Falcon BMS Shared Memory to read live flight data and simulator paths.
*   **Web Server:** Serving a modern Single-Page Application (SPA) frontend to any device on the local network. **(New)** Now supports Progressive Web App (PWA) installation for a more native-like experience on mobile and desktop.
*   **API Provider:** Exposing a REST API (`/api/health`, `/api/kneeboards/*`, **`/api/briefing/html`**) and a WebSocket (`/ws/flight_data`).
*   **Configuration Consumer:** The server is a **consumer** of the `Server/config/settings.json` file. It reads this configuration on demand to understand what content to serve.
*   **(New) HTML Briefing Parser:** Finds the latest mission briefing HTML file, parses its content, and serves a cleaned-up version to the web client.

**Key Implementation Details:**
*   **FastAPI:** Chosen for its high performance and async capabilities.
*   **Autonomous:** Bundled into a single `.exe` with PyInstaller. It supports command-line arguments like `--hide-console`.
*   **User Content Storage:** All user-provided files (images, PDFs for kneeboards) are copied by the launcher into a dedicated, safe directory: `Server/user_data/kneeboards/`.
*   **(New) Centralized Logging:** All server logging is configured via a dedicated `log_config.yaml` file passed directly to the Uvicorn server. This ensures robust, consistent logging to both the console (with color) and a clean text file for the launcher to read.

## Interaction Flow (Updated)

1.  **(Configuration Phase)** The user interacts with the **C# Launcher** to add, remove, and reorder kneeboard pages.
2.  The **Launcher** saves this configuration directly into `Server/config/settings.json` and copies the user's files into `Server/user_data/kneeboards/`.
3.  **(Process Start Phase)** The user clicks "Start Server". The **C# Launcher** starts the **Python Server** process.
4.  The **Launcher's UI** begins polling the Server's `/api/health` endpoint and monitoring `bms_bridge.log` to display its status.
5.  **(Web Client Phase)** The user opens a **Web Browser** on a phone/tablet. The browser may prompt to **"Install App" / "Add to Home Screen"** due to PWA support.
6.  The **Server** serves the main `index.html`, `app.js`, and the Service Worker (`sw.js`).
7.  The **JavaScript App** in the browser makes requests to the server's API endpoints (e.g., `/api/kneeboards/left`, `/api/briefing/html`).
8.  The **Server** reads its configuration, finds the requested files (user kneeboards, BMS briefings), processes them, and sends the data back to the browser.
9.  **(Process Stop Phase)** When finished, the user uses the **C# Launcher** to terminate the **Python Server** process.