# Future Roadmap & Long-Term Vision

This document outlines planned features and improvements for future development cycles, to be addressed after the current core functionality enhancements are complete.

## High-Priority Features (Backlog)

### Kneeboard Management UI

**Concept:**
The single most requested feature is to allow users to customize their kneeboards directly from the launcher's UI. This involves moving beyond the default, auto-detected kneeboards and allowing users to add their own content.

**High-Level Requirements:**
*   A dedicated tab in the C# launcher with two lists representing the Left and Right kneeboards.
*   Functionality to add/remove custom images (PNG, JPG) and PDF documents.
*   Drag-and-drop functionality to easily reorder pages within and between kneeboards.
*   The launcher will save this configuration to a `kneeboard_manifest.json` file.
*   The Python server will read this manifest to build the kneeboard sets, including serving the user-provided files through a secure API endpoint.

**Security Consideration:** This feature requires careful implementation to prevent security vulnerabilities like Path Traversal. The server must only serve files explicitly listed in the trusted, server-side `kneeboard_manifest.json`.

### Settings Window UI

**Concept:**
To support the externalization of configuration (see `current.md`), a user-friendly settings window is required.

**High-Level Requirements:**
*   A new form in the C# project, accessible via the "Settings" (⚙️) button.
*   UI controls to manage all key settings:
    *   Falcon BMS version and installation path (with an auto-detect button).
    *   Server IP/Host and Port.
    *   Kneeboard DDS file numbers and scaling options.
*   Changes made in this window will be saved to the `settings.json` file used by the Python server.

### QR Code Generator

**Concept:**
To simplify connecting mobile devices to the server, the launcher will display a QR code containing the server's local network address.

**High-Level Requirements:**
*   A small, simple form that displays a QR code image.
*   The QR code will encode the URL displayed in the launcher's header (e.g., `http://192.168.1.10:8000`).
*   This will be implemented using a standard C# library for QR code generation.

## Long-Term & Strategic Goals

### Automated Testing

**Task:**
To improve project stability and maintainability, a comprehensive test suite needs to be developed.

**Plan:**
1.  **Unit Tests (Python):** Implement `pytest` tests for the most critical and complex parts of the backend:
    *   The `BriefingParser` logic.
    *   The `CircuitBreaker` implementation in the `bms_adapter`.
    *   API endpoint responses using FastAPI's `TestClient`.
2.  **UI Automation Tests (C#):** Investigate the use of frameworks like `FlaUI` to create automated tests that simulate user interaction with the launcher, verifying UI behavior.

*(This task was logged via the "Remember" protocol).*

### Localization

**Task:**
Refactor the C# launcher to support multiple interface languages.

**Plan:**
*   Move all hardcoded strings from the C# code (`Form1.cs` and `Form1.Designer.cs`) into `.resx` resource files.
*   Implement a mechanism within the application to allow the user to select their preferred language.

### CI/CD Pipeline

**Task:**
Automate the build, testing, and release process.

**Plan:**
*   Connect the GitHub repository to a CI/CD service like GitHub Actions.
*   Create a workflow that, on every new tag (e.g., `v0.2.0`):
    1.  Runs the Python unit tests.
    2.  Builds the C# launcher.
    3.  Runs the PyInstaller build for the server.
    4.  Assembles the final `Release` directory.
    5.  Packages the `Release` folder into a `.zip` archive.
    6.  Automatically creates a new "Release" on GitHub and attaches the `.zip` archive to it.

*(This task was logged via the "Remember" protocol, related to connecting the project to GitHub).*