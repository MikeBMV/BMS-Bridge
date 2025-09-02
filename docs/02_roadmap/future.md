# Future Roadmap & Long-Term Vision

This document outlines planned features and improvements for future development cycles.

## High-Priority Features (Backlog)

### Kneeboard Management UI
**Concept:**
Allow users to customize their kneeboards by adding their own images and PDF documents.
**Requirements:**
*   A UI in the C# launcher with two lists (Left/Right).
*   Functionality to add/remove/reorder (drag-and-drop) pages.
*   The launcher will save the configuration to a `kneeboard_manifest.json`.
*   The Python server will read this manifest and securely serve the user-provided content via a protected API.

### Settings Window UI
**Concept:**
Create a user-friendly GUI to manage all the settings that have been externalized from the code.

### QR Code Generator
**Concept:**
Simplify mobile device connection by displaying a QR code of the server's IP address.

## Strategic Goals

### Automated Testing
**Task:**
Improve project stability and maintainability by implementing a test suite.
**Plan:**
1.  **Unit Tests (Python):** Use `pytest` for critical modules, especially the `BriefingParser` (after its refactoring) and the new `BMSDataSource` abstraction layer (using a `MockBMSDataSource`).
2.  **Integration Tests (Python):** Use FastAPI's `TestClient` to verify API endpoint behavior.
3.  **System (E2E) Tests (C#):** Use a framework like `FlaUI` to automate testing of complex GUI behaviors (e.g., "minimize to tray").

*(Task logged via the "Remember" protocol).*

### Localization
**Task:**
Refactor the C# launcher to support multiple interface languages using `.resx` files.

### CI/CD Pipeline
**Task:**
Automate the build, testing, and release process.
**Plan:**
*   Set up GitHub Actions to run tests on every `push`.
*   On a new tag (e.g., `v0.2.0`), the action should automatically build the project, package the `Release` folder into a `.zip`, and create a release on GitHub.

*(Task logged via the "Remember" protocol).*