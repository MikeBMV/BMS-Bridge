# Future Roadmap & Long-Term Vision

This document outlines planned features and improvements for future development cycles.

## High-Priority Features (Backlog)

### HTML Mission Briefing Viewer
**Status:** Implemented
**Concept:**
A new "Briefing" tab in the web UI displays the full mission briefing, parsed and styled for optimal readability on a second screen. This replaces the initial plan of parsing the `.txt` briefing, which was more complex.
**Requirements (as implemented):**
*   A new API endpoint (`/api/briefing/html`) on the Python server.
*   A dedicated `PathService` that dynamically finds the Falcon BMS `User/Briefings` directory. It prioritizes the live path from Shared Memory (if the simulator is in 3D) and falls back to reading the installation path from the Windows Registry for offline access.
*   The server automatically finds the most recent `.html` briefing file in that directory.
*   The server parses the file using the BeautifulSoup library to cleanly extract the content of the `<body>` tag.
*   The server modifies the HTML on the fly to add custom CSS classes to tables and specific cells, enabling targeted styling for improved readability (e.g., allowing word wrap in "Comments" columns).
*   The web client fetches this cleaned and enhanced HTML and displays it in a scrollable, styled view that is optimized for tablets and phones.

### Progressive Web App (PWA) Support
**Status:** Implemented
**Concept:**
The web interface can be "installed" on desktop and mobile devices, making it behave more like a native application for easier and faster access.
**Requirements (as implemented):**
*   A Web App Manifest (`site.webmanifest`) file is served, containing metadata such as the app's name, description, and icons.
*   A full set of icons for various platforms (Android, iOS, Windows) is provided to ensure a high-quality appearance when added to the home screen.
*   A Service Worker (`sw.js`) is registered by the browser. It caches the core application shell (`index.html`, `app.js`, `style.css`, etc.), which allows the app to load instantly on subsequent visits.
*   Modern browsers on both desktop (Chrome, Edge) and mobile (Android Chrome, iOS Safari) will prompt the user to "Add to Home Screen" or "Install App".
*   When launched from its icon, the application opens in its own standalone window, without the browser's address bar and UI, creating a more immersive, native-like experience.

### Kneeboard Management UI
**Status:** Implemented
**Concept:**
A user-friendly interface inside the C# launcher allows users to fully customize their kneeboards for both the left and right side. Users can add, remove, reorder, and enable/disable any image (`.png`, `.jpg`) or multi-page PDF document.
**Requirements (as implemented):**
*   A dedicated "Kneeboard Management" tab in the C# launcher, featuring a clean `ToolStrip` header for primary controls.
*   Two `ListView` controls for the Left and Right kneeboards, displaying the file paths of the configured pages.
*   Functionality to **Add** new files. The launcher prompts the user to select a file and then copies it into a centralized `Server/user_data/kneeboards/` directory to ensure stability and prevent issues with broken links.
*   Functionality to **Delete** selected pages from the lists.
*   Functionality to **Reorder** items within a list (Move Up/Down) and **Transfer** items between the left and right lists.
*   A checkbox for each item to **Enable/Disable** it without deleting it from the list, allowing users to temporarily hide pages.
*   All changes are immediately and automatically saved to the central `Server/config/settings.json` file.
*   The Python server reads this configuration to display the correct, filtered, and ordered pages in the web UI.

### Settings Window UI
**Status:** Planned
**Concept:**
Create a user-friendly GUI to manage all the settings that have been externalized from the code. This will replace the need for users to manually edit the `settings.json` file for advanced configuration.
**Requirements (Planned):**
*   A new "Settings" window accessible from the main launcher UI (via the `ToolStrip` button).
*   Controls to edit key server parameters like the host IP, port, and polling intervals.
*   A mechanism to specify the Falcon BMS installation path manually, as a fallback for the registry detection.
*   Validation to prevent users from entering invalid values (e.g., a non-numeric port).
*   Changes are saved directly to `Server/config/settings.json`.

### QR Code Generator
**Status:** Implemented
**Concept:**
Simplify mobile device connection by displaying a QR code of the server's IP address. A dedicated button in the launcher's `ToolStrip` opens a window with the generated code for easy scanning.

## Strategic Goals

### Improve Kneeboard Management UX
**Status:** Planned
**Concept:**
Enhance the Kneeboard management tab with more intuitive and efficient controls, reducing the number of clicks required to organize pages.
**Requirements (Planned):**
*   Implement **Drag & Drop** to allow users to reorder items within a single `ListView` by dragging them up or down.
*   Implement **Drag & Drop** to allow users to transfer items between the Left and Right kneeboard lists by dragging an item from one list and dropping it onto the other.
*   Implement **Drag & Drop** to allow users to add new image and PDF files by dragging them directly from Windows Explorer and dropping them onto the desired list.

### Automated Testing
**Status:** Planned
**Task:**
Improve project stability and maintainability by implementing a test suite.
**Plan:**
1.  **Unit Tests (Python):** Use `pytest` for critical modules, especially the `PathService` (with a mocked registry and filesystem) and the HTML parsing logic in the briefing endpoint.
2.  **Integration Tests (Python):** Use FastAPI's `TestClient` to verify the behavior, responses, and error codes of all API endpoints.
3.  **System (E2E) Tests (C#):** Use a framework like `FlaUI` to automate testing of complex GUI behaviors like Kneeboard management (adding, deleting, reordering items) and verify that `settings.json` is updated correctly.

### Localization
**Status:** Planned
**Task:**
Refactor the C# launcher to support multiple interface languages using `.resx` resource files, making the application accessible to a wider international audience.

### CI/CD Pipeline
**Status:** Planned
**Task:**
Automate the build, testing, and release process using GitHub Actions.
**Plan:**
*   Set up a workflow that triggers on every `push` to the `main` branch. This workflow will automatically run all `pytest` unit and integration tests for the Python server.
*   On a new version tag (e.g., `v0.2.0`), a separate workflow should trigger that:
    1.  Runs all tests.
    2.  Builds the C# launcher in `Release` mode.
    3.  Runs the PyInstaller build for the Python server.
    4.  Assembles the complete `Release` directory structure.
    5.  Packages the entire `Release` folder into a `.zip` archive.
    6.  Creates a new Release on GitHub and uploads the `.zip` archive as an asset.