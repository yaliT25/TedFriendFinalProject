# Agent Rules - TED Friend Final Project

> This file defines strict rules for any AI coding agent working on the TED Friend final project.  
> The project is in its final stages. The agent is allowed to perform requested fixes only.  
> When in doubt - stop, explain, and ask for explicit approval before changing anything.

---

## 1. Core Mindset

### 1.1 This is a controlled final-stage project
- Treat the project as a mature final-project codebase, not as a playground for refactoring or redesign.
- The goal is to fix, stabilize, and polish - not to reinvent the product.
- Every change must be small, intentional, reversible, and directly connected to the user's request.

### 1.2 Do not assume
- State assumptions explicitly before implementing.
- If there are several possible interpretations, ask the user which one is correct.
- Do not silently choose an interpretation when the wrong choice may affect UX, UI, database, authentication, OpenAI logic, or deployment.

### 1.3 No speculative work
- Do not add features that were not explicitly requested.
- Do not “improve” flows, copywriting, visual hierarchy, database structure, authentication, or AI prompts unless the user explicitly asked for it.
- If you notice something that should be improved, write a short recommendation and wait for approval.

---

## 2. Approval Rules

### 2.1 Changes that always require explicit user approval
The agent must ask before doing any of the following:

- Changing UX flow, screen structure, navigation, steps, or user journey.
- Changing UI design, layout, spacing, colors, typography, components, animations, icons, or visual hierarchy.
- Changing Hebrew or English copywriting unless the user asked for copy changes.
- Changing the database schema, tables, columns, relationships, seed data, migrations, or existing stored data.
- Changing Google authentication or user-management logic.
- Changing OpenAI, Whisper, or AI prompt behavior beyond the exact requested fix.
- Deleting files, assets, database records, media, or code blocks.
- Moving or renaming files, components, folders, classes, methods, or endpoints.
- Adding new packages, libraries, frameworks, languages, services, or external dependencies.
- Changing deployment configuration or server-related settings.
- Refactoring code that is not directly related to the requested change.

### 2.2 Approval wording
Before any sensitive change, use this structure:

```text
I found an issue that may require changing [area].
Current behavior: [brief description]
Suggested change: [brief description]
Risk: [brief description]
Do you approve this change? I will not implement it until you confirm.
```

---

## 3. Safety and File Protection

### 3.1 Never overwrite critical work
- Never overwrite existing user files without first reading them and understanding their purpose.
- Never replace a whole file when a small targeted edit is enough.
- Never delete code “because it seems unused” unless the user explicitly approves.
- Never remove comments, TODOs, disabled code, assets, or configuration without approval.

### 3.2 Backup before risky edits
Before changing any critical file, create a backup copy or make sure the change can be reverted with git.
Critical files include:
- Database files and schema files.
- Authentication files.
- Configuration files.
- OpenAI / Whisper integration files.
- Main layout, routing, and shared UI components.
- Dashboard components.
- Chat flow components.
- Deployment-related files.

### 3.3 Work with diffs
- Before editing, identify exactly which files must change.
- After editing, summarize the diff in plain language.
- Mention every file changed and why it was changed.
- Do not hide unrelated changes.

---

## 4. Scope of Work

### 4.1 Surgical changes only
- Touch only the files required for the requested fix.
- Prefer the smallest possible change that solves the problem.
- Do not refactor adjacent code.
- Do not normalize formatting in unrelated sections.
- Do not rename variables, functions, classes, or components unless required for the fix.

### 4.2 Keep It Simple — Minimal Code First

The agent must solve every requested issue with the smallest, simplest, and safest code change possible.

Core rules:
- Write minimal, readable code.
- Always prefer a simple, explicit solution over a clever or overly abstract one.
- If the same issue can be solved with 100 lines of clear code instead of 200 lines, choose the 100-line solution.
- Do not add extra abstractions, helper methods, classes, services, or reusable components unless they are truly required for the requested fix.
- Do not introduce architecture changes without explicit user approval.
- Do not write “future-proof” code for scenarios that were not requested.
- Touch the minimum number of files, functions, components, and lines of code required.
- If a solution becomes too large or requires significantly more code than expected, stop before implementing and explain why.

The correct solution is the smallest solution that is clear, safe, and solves the requested issue.

### 4.3 No hidden cleanup
Do not perform unrelated cleanup such as:
- Removing unused imports in unrelated files.
- Reformatting entire files.
- Reorganizing folders.
- Rewriting components.
- Changing CSS globally.
- Updating dependencies.

---

## 5. Technology Constraints

### 5.1 Allowed development technologies
- The project must remain within the technologies learned in the degree program.
- The project is developed in C# and Blazor.
- Do not introduce PHP, Node.js, Python, or any other unapproved backend language or technology.
- User management must be developed only in Blazor, based on the required faculty template.

### 5.2 Database
- Use SQLite only.
- Do not switch to another database.
- Do not change the database schema without explicit user approval.
- Do not write migrations, alter tables, delete records, or reset data without explicit approval.
- If a requested fix appears to require a database change, stop and ask first.

### 5.3 Unity, if relevant
- Unity publication is allowed for web only.
- Do not suggest or implement native mobile apps.

### 5.4 Media and file-size limits
Respect the faculty deployment limitations:
- GIF files: maximum 500KB.
- Images: maximum 300KB.
- MP3 files: maximum 1MB.
- MP4 files: maximum 1MB per file and maximum 5MB total for all videos.
- Word, PowerPoint, and similar files: maximum 500KB.
- Do not store project files in cloud services. Files must be uploaded to the faculty server when required.
- Delete unused images only after explicit approval.
- If users can upload images, reduce image size according to the course method.

### 5.5 OpenAI and paid APIs
- OpenAI and paid API usage must follow the faculty procedure.
- Do not add new paid API calls without approval.
- Do not change model, endpoint, token usage, or API flow without approval.
- Avoid unnecessary API calls.

---

## 6. Authentication and Security

### 6.1 Google login
- The project uses Google login.
- Do not change the authentication mechanism without explicit approval.
- Do not weaken authentication or authorization checks.
- Do not expose user emails, tokens, IDs, conversation data, API keys, or secrets.

### 6.2 Secrets and configuration
- Never hard-code API keys, client secrets, passwords, tokens, or connection strings.
- Do not print secrets to the console, UI, logs, browser dev tools, or error messages.
- Keep secrets in the approved configuration mechanism only.
- If a secret is found in code, report it and ask before changing anything.

### 6.3 User data
- The system stores or processes student interaction data with the AI and shows relevant information to the teacher/dashboard.
- Treat all student data as sensitive learning data.
- Do not expose one student's data to another student.
- Do not add new data collection without approval.
- Do not change what the teacher sees in the dashboard unless explicitly requested.

---

## 7. UX and UI Protection

### 7.1 No UX/UI changes without permission
- Do not change UI or UX unless the user explicitly asks.
- This includes visual design, layout, copy, order of steps, buttons, icons, colors, spacing, responsive behavior, and microcopy.
- If a technical fix may affect UI/UX, ask before implementing.

### 7.2 Audience awareness
The interface serves two different audiences:
- Students in grades 4-6 who interact with the AI coach.
- The teacher/instructor who uses the dashboard.

Every change must preserve this distinction:
- Student-facing language must be simple, encouraging, age-appropriate, and clear.
- Teacher-facing language may be more professional and data-oriented.
- Do not mix teacher terminology into the student experience.
- Do not expose internal technical errors to children.

### 7.3 Hebrew and English interface
- The project combines Hebrew and English.
- Preserve correct directionality in HTML and CSS.
- Use `dir="rtl"` for Hebrew contexts when needed.
- Use `dir="ltr"` for English text, code-like values, email addresses, and technical strings when needed.
- Check mixed Hebrew-English UI carefully after every copy or layout change.

### 7.4 Responsive behavior
- The interface must remain responsive across a wide range of devices.
- Do not hard-code sizes that break mobile, tablet, or desktop views.
- Do not fix a layout on one screen size while breaking another.
- After any UI-related change, check at least desktop and mobile widths.

---

## 8. Accessibility Rules

### 8.1 General accessibility
- Preserve and improve accessibility where directly relevant to the requested change.
- Do not remove keyboard navigation support.
- Do not remove labels, `aria-*` attributes, focus states, or semantic HTML.
- Make sure interactive elements can be reached with the keyboard.
- Maintain visible focus indicators.

### 8.2 Images
- Every image in the interface must include an `alt` attribute.
- If the image is meaningful, write a short descriptive alt text.
- If the image is purely decorative, use `alt=""` when appropriate.
- If unsure what the alt text should be, ask the user.

### 8.3 Forms and buttons
- Inputs must have clear labels.
- Buttons must have understandable text.
- Avoid icon-only buttons unless they have accessible labels.
- Error messages should be clear and connected to the relevant input or action.

### 8.4 Hebrew gender-neutral language
- Hebrew copywriting must be gender-neutral whenever addressing the user.
- Prefer plural or inclusive phrasing when natural.
- Avoid masculine-only or feminine-only wording.
- Examples:
  - Prefer: `בחרו`, `כתבו`, `הוסיפו`, `נסו שוב`.
  - Avoid: `בחר`, `כתוב`, `אתה`, `את` when addressing a general user.
- If a specific phrasing is unclear, ask before changing it.

---

## 9. AI and Whisper Failure Handling

### 9.1 Never crash the UI
If an OpenAI, Whisper, transcription, or AI-related API call fails:
- Do not crash the UI.
- Do not leave the interface loading forever.
- Do not show raw technical errors to students.
- Set the relevant operation/source status to `Failed` when such a status exists.
- Show a friendly message and allow retry when relevant.

### 9.2 Student-facing error message
For the student chat experience, use this friendly message when TED Friend cannot answer:

```text
TED Friend לא יכול לענות כרגע, נסו שוב מאוחר יותר.
```

If there is an existing retry action, the button text should be:

```text
נסו שוב
```

### 9.3 Error details
- Technical details may be logged only in a safe developer log.
- Do not expose stack traces, API responses, keys, tokens, prompts, or internal exception details in the UI.
- Avoid logging sensitive user conversation content unless already approved by the project design.

---

## 10. Code Style and Comments

### 10.1 Comments
- Add concise comments to variables and functions when the code is not self-explanatory.
- Comments must be understandable to a non-computer-science person where possible.
- Do not over-comment obvious code.
- Do not add long theoretical explanations inside code.

### 10.2 Naming
- Preserve existing naming conventions.
- Do not rename existing variables, functions, components, classes, or endpoints unless necessary.
- New names should be clear, simple, and consistent with the current codebase.

### 10.3 Readability
- Prefer explicit readable logic over compact clever logic.
- Keep methods short where possible.
- Do not duplicate large blocks of code.
- If duplication already exists, do not refactor it unless the requested fix requires it.

---

## 11. Testing and Verification

### 11.1 Before coding
Before making changes, the agent must:
- Identify the files likely to be affected.
- Explain the planned change briefly.
- Identify whether approval is required.

### 11.2 After coding
After making changes, the agent must:
- Run relevant build or compile checks if available.
- Run relevant tests if available.
- Manually reason through the affected user flow.
- Check for regressions in nearby behavior.
- Summarize what was changed, where, and why.

### 11.3 For UI-related changes
If the user explicitly approved a UI change:
- Check desktop and mobile layouts.
- Verify RTL/LTR behavior when relevant.
- Verify keyboard navigation.
- Verify image alt text.
- Verify that child-facing text remains age-appropriate.

### 11.4 For database-related changes
If the user explicitly approved a database change:
- Backup the database first.
- Explain the schema/data impact.
- Verify existing data is not lost.
- Provide a rollback plan.

---

## 12. Deployment Constraints

### 12.1 Faculty server readiness
- The project must be able to run on the faculty servers according to the official final-project limitations.
- Do not add technologies, storage methods, media files, or services that may prevent deployment.
- Do not rely on cloud storage for project files.
- Check file-size limits before adding or replacing assets.

### 12.2 Deployment-sensitive files
Do not change deployment-sensitive files without approval, including:
- Configuration files.
- Environment settings.
- Server paths.
- Database location/configuration.
- Authentication configuration.
- API configuration.

---

## 13. Communication Protocol

### 13.1 Before implementation
For every task, respond with:

```text
I understood the request as: [summary].
Files I expect to touch: [list].
Risk level: Low / Medium / High.
Approval needed before coding: Yes / No.
```

If approval is needed, wait.

### 13.2 After implementation
After implementation, respond with:

```text
Done.
Changed files:
- [file]: [what changed]

What I checked:
- [build/test/manual check]

Notes / risks:
- [anything important]
```

### 13.3 When blocked
If blocked, do not improvise. Respond with:

```text
I cannot continue safely because [reason].
I need your decision on [specific question].
```

---

## 14. Absolute Do-Not Rules

The agent must never:
- Delete files without explicit approval.
- Change UX/UI without explicit approval.
- Change database schema without explicit approval.
- Change Google login without explicit approval.
- Add unapproved technologies or languages.
- Add unapproved external dependencies.
- Expose API keys, secrets, tokens, or private user data.
- Leave AI calls in an infinite loading state.
- Show raw technical errors to children.
- Refactor unrelated code.
- Implement features the user did not request.
- Store files in cloud services instead of the faculty server.
- Ignore faculty media and file-size limitations.

---

## 15. Default Decision Rule

When the agent is unsure, the correct action is always:

```text
Stop. Explain the uncertainty. Ask the user before changing anything.
```

