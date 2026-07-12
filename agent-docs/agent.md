# Agent Tasks Workflow — TED Friend

## 1. Purpose
This file defines how the agent should approach every task in the TED Friend project.

The project is in its final correction stage. The agent must work carefully, minimally, and transparently.

## 2. Before Writing Code
Before making any change, the agent must:
1. Read the user’s request carefully.
2. Identify the smallest possible change that solves the request.
3. Identify which files are likely to be affected.
4. State any assumptions if the request is unclear.
5. Ask before continuing if the change may affect a sensitive area.

Sensitive areas include:
- database schema;
- authentication;
- AI prompts or AI request structure;
- routes and API response structures;
- student learning flow;
- instructor dashboard logic;
- UI/UX redesign;
- project architecture.

## 3. During Implementation
The agent must:
- make surgical edits only;
- touch the minimum number of files;
- avoid unrelated refactors;
- avoid future-proofing for scenarios that were not requested;
- avoid adding new dependencies;
- follow `design.md` for UI changes;
- follow `accessibility.md` for UI and interaction changes;
- follow `ai-integration.md` for AI-related changes;
- follow `controllers.md` for controller changes;
- follow `code-comments.md` for comments.

## 4. When to Stop and Ask
The agent must stop and ask for approval before implementation if:
- the change is larger than expected;
- the change requires touching many files;
- the change requires a database schema update;
- the change may alter login or permissions;
- the change may alter AI behavior or prompts;
- the change may alter the student flow or dashboard logic;
- the agent is unsure between multiple possible solutions;
- the agent wants to improve something that was not requested.

## 5. Minimal Code Rule
The agent must always prefer the smallest clear and safe implementation.

If the same problem can be solved with fewer files, fewer functions, or fewer lines of readable code, choose the smaller solution.

Do not add abstractions, helper classes, services, or reusable patterns unless they are truly required for the requested fix.

## 6. After Implementation
At the end of each task, the agent must report:
- what was changed;
- which files were changed;
- what was intentionally not changed;
- whether any assumptions were made;
- what the user should manually test.

## 7. Required Final Report Format
Use this structure at the end of every completed task:

```md
## Summary
Briefly describe the change.

## Files changed
- `path/to/file`
- `path/to/file`

## Not changed
Briefly mention any related areas that were intentionally left untouched.

## Manual checks
- Check X
- Check Y
```

## 8. Main Rule
The agent’s goal is not to improve the whole project.

The agent’s goal is to complete the specific requested task with the least possible risk.
