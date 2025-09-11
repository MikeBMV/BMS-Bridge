# Architectural Decision Record (ADR)

This document records the key architectural decisions made during the project's development.

---
<!-- ADRs 001-005 без изменений -->
### ADR-001: ...
### ADR-002: ...
### ADR-003: ...
### ADR-004: ...
### ADR-005: ...
---

### ADR-006: Centralized Kneeboard Configuration via `settings.json`

**Context:**
The initial concept for kneeboards involved the Python server automatically detecting files. This approach was brittle, inflexible, and not user-friendly.

**Decision:**
To implement a **user-driven, centralized configuration system**.
1.  The C# launcher provides a full UI for managing kneeboard pages.
2.  The launcher **copies** all user-provided files into a local `Server/user_data/kneeboards/` directory.
3.  The complete kneeboard configuration is stored directly within `Server/config/settings.json`.
4.  The Python server acts as a **pure consumer** of this configuration.

**Rationale:**
*   **Robustness:** Storing files locally prevents issues with users moving or deleting original source files.
*   **Flexibility:** Gives the user full control over content and presentation order.
*   **Clear Separation of Concerns:** The launcher is the "manager" of the configuration, and the server is the "executor."

---

### ADR-007: Refactoring the Web UI to a Single Universal Canvas Viewer

**Context:**
The initial web UI implementation used different HTML elements for different content types (`<img>` for images, `<canvas>` for PDFs). This led to rendering conflicts and race conditions.

**Decision:**
The web UI was refactored to use a **single, universal `<canvas>` element** for displaying *all* content types.

**Rationale:**
*   **Elimination of Rendering Conflicts:** Using only one rendering technology eliminates browser-level conflicts.
*   **Code Simplification (DRY):** All logic is centralized in a single rendering function.
*   **Unified Feature Development:** Future features like zoom or rotation only need to be implemented once.

---

### ADR-008: Refactoring Launcher UI to a ToolStrip Header

**Status:** Implemented

**Context:**
The initial UI for the C# launcher used a `Panel` docked at the top of the window to display status information and control buttons. This design consumed a significant amount of vertical screen space and looked dated.

**Decision:**
To completely remove the header `Panel` and replace it with a single `ToolStrip` control docked to the top of the main window.

**Rationale:**
*   **Space Efficiency:** This change significantly reduces the vertical footprint of the non-functional UI, giving more space to the primary content (the Kneeboard management tabs).
*   **Modern Look and Feel:** A `ToolStrip` provides a cleaner, more integrated, and professional appearance, similar to the UI conventions of modern applications like Visual Studio Code or web browsers.
*   **Consolidation:** All status labels (Server Status, BMS Status, Server Address) and control buttons (Start/Stop, Settings, QR Code) are now consolidated into a single, logical control bar.

---

### ADR-009: Centralizing Server Logging via Uvicorn

**Status:** Implemented

**Context:**
The initial server logging implementation was a fragile mix of the `structlog` and standard `logging` libraries. This led to configuration conflicts with the Uvicorn web server, resulting in an unreliable pipeline where logs appeared in the console but were not written to the file that the C# launcher monitors.

**Decision:**
To completely delegate logging configuration to the Uvicorn server by implementing a dedicated `log_config.yaml` file.

**Rationale:**
*   **Single Source of Truth:** The `log_config.yaml` becomes the single, unambiguous source for all logging rules, eliminating conflicts. This is the recommended practice for FastAPI/Uvicorn applications.
*   **Robustness:** This approach correctly captures logs from all sources (Uvicorn's access/error logs, our application's logs) and reliably directs them to multiple destinations.
*   **Flexibility:** The new configuration easily supports different formatters for different outputs, allowing for colored logs in the developer console and clean, timestamped text logs in the file for the launcher.