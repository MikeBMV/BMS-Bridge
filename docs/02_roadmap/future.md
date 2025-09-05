# Future Roadmap & Long-Term Vision

This document outlines planned features and improvements for future development cycles.

## High-Priority Features (Backlog)

### Kneeboard Management UI
**Status:** Implemented

**Concept:**
A user-friendly interface inside the C# launcher allows users to fully customize their kneeboards for both the left and right side. Users can add, remove, reorder, and enable/disable any image (`.png`, `.jpg`) or multi-page PDF document.

**Requirements (as implemented):**
*   A dedicated "Kneeboard Management" tab in the C# launcher.
*   Two `ListView` controls for the Left and Right kneeboards, displaying the file paths.
*   Functionality to **Add** new files. The launcher copies the selected file into a centralized `Server/user_data/kneeboards/` directory to ensure stability.
*   Functionality to **Delete** files from the lists.
*   Functionality to **Reorder** items (Move Up/Down) and **Transfer** items between the left and right lists.
*   A checkbox for each item to **Enable/Disable** it without deleting it from the list.
*   All changes are immediately and automatically saved to the central `Server/config/settings.json` file.
*   The Python server reads this configuration to display the correct, filtered, and ordered pages in the web UI.

### Settings Window UI
**Status:** Planned
**Concept:**
Create a user-friendly GUI to manage all the settings that have been externalized from the code.

### QR Code Generator
**Status:** Implemented
**Concept:**
Simplify mobile device connection by displaying a QR code of the server's IP address. A dedicated button in the launcher opens a window with the generated code.

## Strategic Goals

### Automated Testing
**Task:**
Improve project stability and maintainability by implementing a test suite.
**Plan:**
1.  **Unit Tests (Python):** Use `pytest` for critical modules, especially the `BriefingParser` and configuration loading logic.
2.  **Integration Tests (Python):** Use FastAPI's `TestClient` to verify API endpoint behavior.
3.  **System (E2E) Tests (C#):** Use a framework like `FlaUI` to automate testing of complex GUI behaviors like Kneeboard management.

### Localization
**Task:**
Refactor the C# launcher to support multiple interface languages using `.resx` files.

### CI/CD Pipeline
**Task:**
Automate the build, testing, and release process.
**Plan:**
*   Set up GitHub Actions to run tests on every `push`.
*   On a new tag (e.g., `v0.2.0`), the action should automatically build the project, package the `Release` folder into a `.zip`, and create a release on GitHub.