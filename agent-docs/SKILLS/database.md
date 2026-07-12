# Database Skill — TED Friend

## 1. Purpose
This skill defines safe rules for working with the SQLite database in the TED Friend project.

The database is a sensitive area. The agent must not modify it unless the user explicitly requests or approves the change.

## 2. Approved Database
The project uses SQLite only.

Do not introduce another database, ORM strategy, cloud database, or external storage service without explicit written approval.

## 3. Schema Changes
The agent must not change the database schema without explicit user approval.

Schema changes include:
- adding a table;
- deleting a table;
- renaming a table;
- adding a column;
- deleting a column;
- renaming a column;
- changing column types;
- changing relationships;
- changing required/optional behavior.

## 4. Before an Approved Database Change
Before implementing an approved database change, the agent must explain:
- which table(s) will be affected;
- which column(s) will be affected;
- which controllers may be affected;
- which DTOs may be affected;
- which UI screens may be affected;
- whether existing data may be at risk;
- what the smallest safe implementation is.

## 5. Data Safety
Rules:
- Do not delete existing data unless explicitly requested.
- Do not write destructive queries unless explicitly requested.
- Do not reset tables or seed data without approval.
- Preserve existing records whenever possible.
- Avoid changes that may break existing saved sessions.

## 6. Query Changes
Query changes are allowed only when needed for the requested task.

Rules:
- Keep queries simple and readable.
- Do not optimize queries speculatively.
- Do not rewrite working database access code unless required.
- If a query affects dashboard data, verify the expected dashboard behavior.
- If a query affects student progress, verify the expected student flow.
- After changing a query, report to the developer what the updated query does and which data it reads, writes, updates, or deletes.

## 7. DTO and Database Alignment
When changing database-related code, check that:
- DTOs still match the returned data;
- controllers still return the expected structure;
- frontend code still reads the expected fields;
- dashboard and student screens are not broken.

## 8. Main Rule
Database changes are allowed only when explicitly requested or approved by the user.

When approval is given, the agent must make the smallest safe database change and preserve existing data.
