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

---

### ADR-004: Refactoring the C# Launcher to Follow SRP

**Context:**
Expert reviews (Grok, ChatGPT, Claude) unanimously pointed out that the `Form1.cs` class was a "God Class" that violated the Single Responsibility Principle (SRP) by mixing UI, process management, and API polling logic.

**Decision:**
The C# launcher was refactored into a cleaner, multi-class architecture.

**Rationale:**
*   **SRP Adherence:** The new architecture separates concerns into distinct classes:
    *   `ServerManager`: Handles all process lifecycle and log monitoring.
    *   `HealthMonitor`: Handles all API polling.
    *   `UIController`: Handles all UI updates.
    *   `Form1`: Acts as a coordinator, wiring the components together.
*   **Testability:** Each component can now be tested in isolation.
*   **Maintainability:** Changes to one area of responsibility (e.g., how the server is stopped) are now confined to a single class, making the code easier to understand and modify.

---

### ADR-005: Server Launch Method - Independent Process with PID File

**Context:**
The previous method of launching the server as an independent process and stopping it via `taskkill /IM` was functional but identified as "brittle" by experts. A more precise control mechanism was needed.

**Decision:**
The `ServerManager` will launch the server as an independent process and immediately save its Process ID (PID) to a `server.pid` file. To stop the server, the manager will read the PID from this file and terminate that specific process and its children.

**Rationale:**
*   **Precision:** Terminating by PID is an exact operation that eliminates the risk of killing an unrelated process that happens to have the same name, which was the main critique of the `taskkill /IM` method.
*   **Robustness:** This approach combines the resilience of running an independent server process with the precise control of direct process management.
*   **State Management:** The `server.pid` file acts as a simple but effective state mechanism, allowing the launcher (or other tools) to know the PID of the running server instance.

---

### ADR-006: Refactoring the Web UI to a Single Universal Viewer

**Context:**
The initial implementation of the web front-end used different technologies for different content types. Images were displayed using the standard `<img>` HTML tag, while PDF documents were rendered onto a `<canvas>` element using the `pdf.js` library. During testing, this led to unpredictable rendering bugs, especially when a user would navigate between pages of different types (e.g., from a PDF to an image). The two elements (`<img>` and `<canvas>`) appeared to conflict when being rapidly hidden and shown within the same container.

**Decision:**
The web UI was refactored to use a **single, universal `<canvas>`-based viewer** for all content types.

**Rationale:**
*   **Elimination of Rendering Conflicts:** By using only one rendering technology (`<canvas>`), we completely eliminate the race conditions and conflicts between competing browser rendering pipelines for `<img>` and `<canvas>`.
*   **Code Simplification:** Instead of having complex logic to manage and toggle the visibility of two different HTML elements, the new architecture uses a "clean room" approach. Before rendering any page, the viewer's content area is completely cleared. Then, a single `<canvas>` element is created from scratch to display the content. This dramatically simplifies the rendering logic.
*   **Unified Feature Development:** This architecture ensures that any future feature (e.g., zoom, rotation) only needs to be implemented once. Since all content is ultimately rendered on a `<canvas>`, the same code will work for both images and PDF pages.
*   **Consistency:** The user experience becomes more consistent, as all document-based views (Kneeboards, Docs, Charts) now use the exact same viewing component, ensuring identical behavior and controls. The logic for drawing an image onto a canvas is trivial and does not introduce significant performance overhead.

---

### ADR-006: Centralized Kneeboard Configuration via `settings.json`

**Context:**
The initial concept for kneeboards involved the Python server automatically detecting and converting `.dds` files from the simulator's installation directory. This approach was brittle, inflexible, and not user-friendly. It did not allow for custom images, PDFs, or reordering.

**Decision:**
To implement a **user-driven, centralized configuration system** for kneeboards.
1.  The C# launcher provides a full UI for adding, removing, reordering, and enabling/disabling kneeboard pages.
2.  The launcher **copies** all user-provided files into a local `Server/user_data/kneeboards/` directory.
3.  The complete kneeboard configuration (lists of files for left and right kneeboards) is stored directly within the server's main `Server/config/settings.json` file.
4.  The Python server acts as a **pure consumer** of this configuration, simply reading the file to determine which content to serve.

**Rationale:**
*   **Robustness:** Storing files within the application's directory prevents issues with users moving or deleting original source files.
*   **Flexibility:** This model easily supports various file types (images, PDFs) and gives the user full control over content and presentation order.
*   **Clear Separation of Concerns:** The launcher is the "manager" of the configuration, and the server is the "executor." This aligns perfectly with the project's overall architecture.
*   **Simplicity:** It removes complex file-watching and conversion logic from the Python server, simplifying its role.

---

### ADR-007: Refactoring the Web UI to a Single Universal Canvas Viewer

**Context:**
The initial web UI implementation used different HTML elements for different content types: `<img>` for images and `<canvas>` for PDFs. This led to rendering conflicts and race conditions when rapidly switching between pages of different types, resulting in blank or broken content.

**Decision:**
The web UI was refactored to use a **single, universal `<canvas>` element** for displaying *all* content types.

**Rationale:**
*   **Elimination of Rendering Conflicts:** By using only one rendering technology, we completely eliminate the browser-level conflicts between the `<img>` and `<canvas>` rendering pipelines.
*   **Code Simplification (DRY):** Instead of managing multiple viewer functions and UI elements, all logic is centralized in a single `_setupUniversalViewer` function. This function uses a "clean room" approach: it clears the content area and creates a fresh `<canvas>` for each page, ensuring a predictable state.
*   **Unified Feature Development:** Future features like zoom or rotation only need to be implemented once for the `<canvas>` element and will automatically work for both images and PDFs.
*   **Consistency:** The user experience is identical across all document-based views (Kneeboards, Docs, etc.), as they all share the exact same underlying component.