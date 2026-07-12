# Technical SPEC — TED Friend (Classi)

## 1. Purpose
This document describes the technical structure and implementation boundaries of the TED Friend project. It is intended to help the agent make safe, minimal, and well-scoped code changes.

The project is in its final correction stage. The agent must avoid broad refactors, architecture changes, or speculative improvements.

## 2. Technology Stack
The project must remain within the technologies approved for the final project.

Allowed stack:
- C#
- Blazor WebAssembly / ASP.NET Core
- SQLite
- Google Login / authentication mechanism already implemented in the project
- OpenAI API / Whisper API may be used only where they are already used in the project, or when explicitly requested by the developer.
- HTML / CSS within Blazor components

Do not introduce new languages, frameworks, backend runtimes, databases, cloud storage services, or external infrastructure without explicit user approval.

## 3. Frontend Structure
The frontend is based on Blazor components.

Rules:
- Keep UI changes scoped to the relevant component.
- Prefer the component’s isolated `.razor.css` file for styling.
- Do not use inline styles in `.razor` files.
- Do not introduce structural layout changes unless requested.
- Preserve the existing student flow and dashboard flow.
- Follow `design.md` for visual and responsive rules.
- Follow `accessibility.md` for accessibility rules.

## 4. Backend Structure
The backend is based on ASP.NET Core controllers and C# logic.

Rules:
- Do not rename routes or endpoints without explicit approval.
- Do not change response structures unless the requested fix requires it and the user approves.
- Keep controller changes small and direct.
- Preserve authentication, authorization, and ownership checks.
- Do not move logic between controllers, services, or components unless explicitly requested.

## 5. Database
The project uses SQLite.

Rules:
- SQLite is the only approved database unless the user provides written approval for another database.
- Do not change the database schema without explicit user approval.
- Do not rename tables or columns without explicit user approval.
- Do not delete data or write destructive queries unless explicitly requested.
- Before any approved database change, explain which tables, columns, DTOs, controllers, and UI screens may be affected.

## 6. Authentication
The project uses Google Login.

Rules:
- Do not replace or redesign the login mechanism.
- Do not weaken security checks.
- Do not expose user identifiers, emails, tokens, or private authentication data in the UI or console.
- Do not change user ownership logic without explicit approval.

## 7. AI Integration
The project uses AI to support the student learning flow.

Rules:
- Do not change system prompts without explicit approval.
- Do not expose system prompts to users.
- Do not change AI model, endpoint, request payload, response parsing, or storage behavior without approval.
- Handle AI failures gracefully.
- Never leave the UI in an infinite loading state.
- Use the approved user-facing fallback message when AI is unavailable:
  `Ted Friend לא יכול לענות כרגע, נסו שוב מאוחר יותר.`

## 8. DTOs
DTOs transfer data between layers such as database operations, controllers, the AI flow, the student interface, and the instructor dashboard.

Rules:
- Do not change DTO property names or shapes unless required by the requested fix.
- If changing a DTO, check all affected usage points.
- Do not add fields “just in case.”
- Follow `code-comments.md` when adding comments to DTO classes.

## 9. Files and Assets
Rules:
- Do not add heavy assets unnecessarily.
- Keep images optimized according to faculty constraints.
- Do not store project files in external cloud services.
- Delete unused assets only when explicitly requested and after confirming they are unused.
- Preserve logos, mascot assets, icons, and backgrounds unless the user asks to change them.

## 10. Testing and Verification
After a change, the agent should report what should be checked manually.

Relevant checks may include:
- student login;
- instructor login;
- topic selection;
- chat flow;
- phase progression;
- AI response failure behavior;
- dashboard data display;
- responsive layout on desktop/tablet/mobile;
- keyboard navigation;
- database save/read behavior.

## 11. Implementation Rule
The correct technical solution is the smallest safe solution that solves the requested issue without changing unrelated behavior.

## Publish and Faculty Server Constraints

This project must remain compatible with the faculty server and final project submission constraints.

The agent must not introduce any technical change that may prevent the project from being published to the faculty server.

Rules:
- Use only the technologies already approved for this project: C#, Blazor, ASP.NET Core, SQLite, Google Login, and OpenAI API where already used or explicitly requested by the developer.
- Do not add new programming languages, frameworks, runtime environments, external servers, or cloud storage services without explicit developer approval.
- Do not replace SQLite with another database.
- Do not change connection strings, authentication settings, JWT behavior, Google Login flow, or server configuration without explicit developer approval.
- Do not add large media files or assets that may exceed faculty upload limits.
- Do not store project files, uploaded files, or user data in external cloud services unless explicitly approved.
- Keep all changes compatible with the existing publish process.
- If a requested change may affect deployment, hosting, authentication, database access, file storage, API access, or server compatibility, stop before implementing and explain the risk to the developer.

Before completing a task that may affect publish or deployment, the agent must report:
1. whether the change affects server compatibility;
2. whether any new dependency, package, file type, or configuration was added;
3. whether authentication, database access, OpenAI API usage, or file storage was affected;
4. what the developer should check before publishing.