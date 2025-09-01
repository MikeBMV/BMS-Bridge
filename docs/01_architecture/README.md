# Architecture Overview

This document describes the high-level architecture of the BMS Bridge project.

## Guiding Principles

The project's architecture is built on two core principles:

1.  **Use the Right Tool for the Job:** We leverage the distinct strengths of both the C#/.NET and Python ecosystems. C# is used for its robust capabilities in creating native Windows GUI applications, while Python is used for its speed in developing web backends and its rich library support for data processing.
2.  **Decoupling and Resilience:** The frontend (Launcher) and backend (Server) are two separate, independent applications. This separation ensures that a failure in one component does not bring down the entire system and allows for independent development and updates of each part.

## Core Components: The "Launcher-Server" Model

BMS Bridge consists of two main applications that work in tandem.

### 1. The C# WinForms Launcher (`BMS_Bridge_Launcher`)

This is a lightweight native Windows application that acts as the primary user interface and control panel.

**Responsibilities:**
*   **Process Management:** Starting and reliably stopping the Python server process.
*   **UI Feedback:** Displaying the real-time status of both the server and the connection to the Falcon BMS simulator.
*   **System Integration:** Providing a familiar user experience through a system tray icon, allowing the application to run unobtrusively in the background.
*   **Configuration Hub:** Serving as the future entry point for all user-facing settings (e.g., Kneeboard Management).

**Key Implementation Details:**
*   It does **not** contain any core business logic (like parsing files or reading simulator data).
*   It interacts with the server as a "black box" through two primary mechanisms:
    1.  **Health API Polling:** A timer periodically sends requests to the server's `/api/health` endpoint to get its status. This is the **single source of truth** for the UI.
    2.  **Process Termination:** It uses the `taskkill /T` system command to ensure the entire process tree created by PyInstaller is terminated correctly.

### 2. The Python FastAPI Server (`Server_Core`)

This is the engine of the project. It's a console application compiled into a standalone `.exe` using PyInstaller, which handles all data processing and communication.

**Responsibilities:**
*   **Data Acquisition:** Connecting to the Falcon BMS Shared Memory to read live flight data.
*   **File Processing:** Locating, parsing, and caching mission briefing files (`briefing.txt`).
*   **Image Conversion:** Discovering, converting (`.dds` -> `.png`), and caching kneeboard chart images.
*   **Web Server:** Serving a modern Single-Page Application (SPA) frontend built with vanilla JavaScript.
*   **API Provider:** Exposing a simple REST API (`/api/health`) and a WebSocket endpoint (`/ws/flight_data`) for communication with the frontend clients (the web interface).

**Key Implementation Details:**
*   **FastAPI Framework:** Chosen for its high performance, modern async capabilities, and automatic documentation features.
*   **Dependency-Free:** Bundled into a single executable with PyInstaller, meaning the end-user requires no Python installation.
*   **Stateless by Design:** The server itself is mostly stateless, relying on configuration files and data from the simulator. State is maintained on the client-side (e.g., in `localStorage`).

## Communication Flow

1.  The **C# Launcher** starts the **Python Server** as an independent process.
2.  The **Launcher's UI** begins polling the Server's `/api/health` endpoint every few seconds.
3.  The **Server** responds with its status (e.g., `RUNNING`, `WARNING`) and its connection status to the simulator.
4.  The user opens a **Web Browser** on a phone/tablet and navigates to the Server's IP address.
5.  The **Server** serves the `index.html` and `app.js` files to the Browser.
6.  The **JavaScript App** in the browser connects to the Server's `/ws/flight_data` WebSocket to receive real-time data and makes calls to its REST API to get briefing/kneeboard information.
7.  When the user is finished, they use the **C# Launcher** to send a `taskkill` command, shutting down the **Python Server**.

This decoupled model ensures a stable and flexible system where each component has a clear and distinct role.