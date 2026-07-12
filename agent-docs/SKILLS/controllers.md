# Controllers Skill

## 1. Purpose

This skill defines how the agent should edit ASP.NET Core controllers in the TED Friend project.

Controllers are sensitive because they may affect authentication, role logic, database access, AI-related flows, student progression, and instructor dashboard data.

The agent must treat controller changes as high-risk and make only the smallest safe change required for the requested task.

## 2. Core Rule

Controller changes must be minimal, safe, readable, and scoped to the specific requested issue.

The agent must not refactor, reorganize, rename, or rewrite controller logic unless the developer explicitly requests or approves it.

The goal is not to improve the whole controller.  
The goal is to solve the requested issue without breaking existing flows.

## 3. Controller Method Structure

When editing controller methods, the agent must preserve the existing controller style and structure.

Controller methods should generally follow this order:

1. Identify the authenticated user when the method depends on the logged-in user.
2. Return `Unauthorized()` if the authenticated user cannot be identified.
3. Validate the required input for the specific request.
4. Open the SQLite connection using `DefaultConnection`.
5. Keep SQL queries simple, readable, and scoped to the requested task.
6. Execute only the database action required by the method.
7. Return a clear DTO, status result, or error response that matches the existing client-side expectations.
8. After changing a controller method or query, briefly report to the developer what the method now does and what data it reads, writes, updates, or deletes.

The agent must not rewrite controller methods into a different architecture unless explicitly approved by the developer.

## 4. Authentication Pattern in Controllers

When a controller method requires the logged-in user, preserve the existing user identification pattern:

```csharp
var userId = User.FindFirst("sub")?.Value 
    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
```

If the user ID is missing, return `Unauthorized()`.

The agent must not change any of the following without explicit developer approval:

- JWT claim logic;
- Google Login flow;
- JWT cookie behavior;
- role logic;
- teacher/student identification logic;
- dashboard access logic;
- authentication-related routes or response structures.

## 5. Routes, Request Bodies, and DTO Responses

The agent must not change route names, HTTP methods, request body structures, response DTOs, or response object structures unless explicitly requested.

Changing a controller response may break:

- the Blazor client;
- the login flow;
- the student learning flow;
- the instructor dashboard;
- existing DTO mapping;
- existing database operations.

If a requested change requires changing a route, request body, or response DTO, the agent must stop and explain the expected impact before implementing.

## 6. SQL Queries Inside Controllers

Query changes are allowed only when needed for the requested task.

Rules:

- Keep queries simple and readable.
- Do not optimize queries speculatively.
- Do not rewrite working database access code unless required.
- Do not introduce complex SQL, broad query rewrites, joins, grouping logic, or nested queries unless required for the requested task and approved when risky.
- If a query affects dashboard data, verify the expected dashboard behavior.
- If a query affects student progress, verify the expected student flow.

After changing a query, the agent must clearly report to the developer:

1. what the updated query does;
2. which table or tables it reads from or writes to;
3. whether it reads, inserts, updates, or deletes data;
4. which controller method uses it;
5. which screen, DTO, or user flow may be affected.

## 7. Database Safety from Controllers

The agent must not make database schema changes from controller work unless explicitly requested or approved by the developer.

The agent must not:

- rename tables or columns;
- add or remove columns;
- delete existing data;
- change database relationships;
- change the meaning of existing values;
- add new persistence behavior;
- change how student progress, teacher linkage, roles, or class codes are stored;

without explicit developer approval.

If controller work reveals a possible need for a database change, the agent must stop and ask before implementing it.

## 8. Dashboard and Student Flow Safety

Controller changes that affect dashboard data or student progression require extra care.

The agent must preserve:

- teacher/student separation;
- class code behavior;
- teacher ownership of student data;
- student progress tracking;
- current phase behavior;
- dashboard counters and student report data;
- help status / self-report data;
- AI feedback data shown in the summary or dashboard.

The agent must not expose instructor dashboard data to students or change dashboard access logic without explicit approval.

If a controller change affects the dashboard, the agent must report what dashboard data may change.

If a controller change affects the student flow, the agent must report which phase or student action may be affected.

## 9. Error Handling

Controller error handling must be safe and user-friendly.

The agent must:

- avoid crashing the UI;
- avoid leaving the interface in an endless loading state;
- return clear status results that the client can handle;
- avoid exposing unnecessary technical details to end users;
- preserve existing user-facing error behavior unless asked to change it.

Technical error details may be useful during development, but the agent must not add overly detailed error exposure to users without approval.

## 10. Controller Comments

Controller comments must follow the `code-comments.md` skill.

Comments should explain the purpose of the method or section for project evaluators.  
They must not describe the agent’s work process, previous fixes, personal instructions, or temporary debugging logic.

Good example:

```csharp
// שיטה זו מחזירה את נתוני המשתמש המחובר לצורך המשך תהליך הכניסה למערכת
```

Bad examples:

```csharp
// כמו שביקשת, תיקנתי את השאילתה
// זה יעזור לך לראות מה הבעיה אם זה קורס
// תיקון זמני
```

## 11. Forbidden Changes Without Approval

The agent must not perform any of the following without explicit developer approval:

- change authentication or Google Login logic;
- change JWT claim handling;
- change role logic;
- change teacher/student access rules;
- change dashboard access rules;
- change route names or HTTP methods;
- change request or response structures;
- change DTO structures used by the client;
- perform broad controller refactors;
- move controller logic into new services or architecture layers;
- add new controller endpoints that were not requested;
- remove existing endpoints;
- change database schema;
- delete or overwrite data;
- rewrite working SQL queries for speculative optimization.

## 12. Completion Report

After editing a controller, the agent must report:

- which controller method was changed;
- which files were changed;
- what the change solves;
- whether authentication, roles, dashboard data, student flow, DTOs, or database queries were affected;
- if a SQL query changed, what the updated query does;
- what was intentionally not changed.
