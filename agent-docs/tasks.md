# TED Friend — Implementation Tasks

This file organizes the remaining implementation tasks for the TED Friend project.

The project is in its final correction stage.  
The agent must not implement multiple tasks at once without explicit developer approval.

---

# Product Tasks

Each product task must be handled as a separate task unless the developer explicitly approves combining tasks.

---

## Batch 1 — Conversation Persistence

### Task 1.1 — Save Conversation History Between Entries

Add support for preserving the student’s AI conversation when the student exits and later returns to the same training topic.

A student may have several conversation sessions under the same presentation topic.  
The system should preserve and display the relevant conversation history for the selected topic when the student returns.

### Expected Change Type

Functional change, not visual.

### Likely Areas

- Database
- Controllers
- DTOs
- Chat / training screen logic
- HTML/Razor display if required

### Rules

- Database changes require explicit developer approval.
- Preserve existing student flow.
- Do not change the pedagogical phase order.
- Do not rewrite existing AI communication logic unless required.
- Report all database and query changes clearly.

---

## Batch 2 — Student Word Summary Product

### Task 2.1 — Generate Personal Word Summary for Student

Add a mechanism for generating a personal Word document at the end of the process.

The document should summarize the presentation parts built during the chat and should rely only on the existing student conversation and data in the system.

AI may help organize the content into a clear structure, but it must not add new ideas that were not created by the student.

### Document Content

Include only the sections that the student has actually completed.

The Word document may include:

1. Student name and presentation topic.
2. Opening — self-introduction, hook, and transition sentence.
3. Body — two main ideas, each with a short explanation or example.
4. Conclusion — recap of the main idea and a closing sentence with a clear takeaway.
5. Voice practice — short summary or status of the voice practice stage, based on saved data.
6. Feedback — summary feedback, practice tips, and help request status.

### Opening Sentence Example

The document may begin with a short personal sentence in Hebrew, such as:

```text
היי [שם התלמיד/ה], הקובץ הזה מסכם את ההתקדמות שעשינו עד כה בפרזנטציה שלך בנושא [נושא הפרזנטציה].
```

### Rules

- Do not invent missing presentation content.
- Do not complete missing phases on behalf of the student.
- Do not create new presentation ideas that were not part of the student’s process.
- If AI is used, follow `ai-integration.md`.
- If file generation requires a new package or dependency, stop and ask before implementing.
- The instructor may also access the same Word product through the instructor dashboard when explicitly requested.

---

## Batch 3 — Voice Practice Data

### Task 3.1 — Save Student Voice Messages

Add support for saving voice messages recorded by the student.

### Expected Change Type

Functional change, not visual.

### Likely Areas

- Database
- File/audio storage logic
- Controllers
- Voice practice screen

### Rules

- Database changes require explicit developer approval.
- Do not add external cloud storage without explicit approval.
- Follow faculty file-size and storage constraints.
- Do not change the voice practice flow without approval.
- Do not expose student recordings to unauthorized users.

---

## Batch 4 — Small Functional and UI Fixes

These tasks are smaller and should be implemented one by one.

### Task 4.1 — Auto Scroll in Chat

Add automatic scrolling behavior where needed in the chat/training flow.

Rules:
- Keep the change minimal.
- Do not redesign the chat screen.
- Do not change message logic.

### Task 4.2 — Required Student Input Field

Make sure the relevant student input field is required and visually marked with a red asterisk.

Rules:
- Keep validation clear and child-friendly.
- Do not block unrelated flow areas.
- Follow accessibility rules for required fields.


### Task 4.3 — Safari Browser Compatibility

Investigate and fix Safari compatibility issues.

Rules:
- Do not add browser-specific hacks unless necessary.
- Explain what Safari issue was found.
- Keep changes minimal and responsive.

### Task 4.4 — Change Tip Text Color to Black

In the student trainings screen, change the tip text color to black.

The student trainings screen is the Blazor page:

```razor
@page "/StudentDashboard"
```

Rules:
- CSS-only change.
- Use the relevant `.razor.css` file.
- Do not change surrounding layout.

---

## Batch 5 — Instructor Dashboard Upgrade

### Task 5.1 — Show Dashboard Rows by Student and Topic

Update the instructor dashboard so each row represents a unique student-topic combination, not every individual session.

Important distinction:

- Do not show every technical session as a separate row.
- A student may have several sessions or conversations under the same presentation topic.
- Conversations/sessions by the same student on the same topic should be grouped into one dashboard row.
- Different topics by the same student should appear as separate rows.
- Different students with the same topic should still appear as separate rows, because the grouping is by student and topic together.

Each row may include:

- student name;
- student email, if already used in the dashboard;
- presentation topic;
- current or latest progress phase for that topic;
- relevant score or average score for that topic, according to existing data;
- help request status for that topic;
- last updated date for that topic.

### Expected Change Type

Functional change that likely affects data structure, query logic, and dashboard display.

### Likely Areas

- Instructor dashboard page/component
- Dashboard DTOs
- Controller method that returns instructor dashboard data
- SQL query or data aggregation logic
- Possibly Word download logic if the Word product is connected to a topic row

### Rules

- Do not treat every ChatSession row as a separate instructor dashboard row unless explicitly requested.
- Group dashboard data by student and presentation topic.
- Do not change dashboard access logic without explicit approval.
- Do not expose dashboard data to students.
- Keep the dashboard professional and instructor-facing.
- Controller and query changes must follow `controllers.md` and `database.md`.
- After changing the query, report exactly what the updated query does and how the grouping works.

### Task 5.2 — Add Filtering and Sorting

Add filtering and sorting to the dashboard using existing system data.

Possible filters may include:

- student;
- topic;
- progress phase;
- score;
- help request status;
- last updated date.

Rules:
- Filtering and sorting should respect the student-topic row grouping from Task 5.1.
- Do not add unnecessary libraries or dependencies without approval.
- Keep the dashboard clear, professional, and useful for instructors.

### Task 5.3 — Download Word Summary Per Topic Row

Add an option to download the Word summary for a specific student-topic row.

The instructor may access the same Word product that the student receives, so no separate short dashboard summary is required.

Rules:
- The Word file should summarize the selected topic row.
- Do not create an additional dashboard-only summary unless explicitly requested.
- Do not expose one student’s Word product to another student.
- Follow `ai-integration.md` if AI is used to organize the Word content.

### Task 5.4 — Export Dashboard Data to Excel

Add an option to export dashboard data to an Excel file where each row represents one student-topic row.

The file may include:

- student data;
- topic;
- scores;
- progress status;
- help requests;
- last updated date.

If possible and approved, a summary chart may also be included or exported.

Rules:
- Excel export should respect the student-topic grouping from Task 5.1.
- If export requires a new package or dependency, stop and ask before implementing.

### Dashboard Rules

- The dashboard is instructor-facing only.
- Do not expose dashboard data to students.
- Do not make the dashboard visually childish.
- Preserve professional, clear, instructor-focused design.
- Do not change dashboard access logic without explicit approval.
- Controller and query changes must follow `controllers.md` and `database.md`.
- This batch is large and should be broken into smaller approved steps before implementation.

---

## Batch 6 — New Training Intro Popup

### Task 6.1 — Replace Terms Popup with New Training Intro Popup

Remove the Terms of Use popup currently shown on the login screen, and add an explanation popup before the start of a new empty training session.

### Display Rules

Show the popup only when:

- the student starts a new training session;
- the training session is empty;
- no AI conversation has started yet.

Do not show the popup when:

- the student returns to an existing training session;
- the student continues a conversation that has already started.

### Popup Content Direction

The popup should explain:

- the purpose of the chat;
- the four main stages of the process;
- how TedFriend guides the student;
- that students can move between stages using the right-side buttons;
- privacy and safety guidance.

### Suggested Hebrew Copy

```text
ברוכים הבאים וברוכות הבאות לשיחה עם TedFriend!

השיחה נועדה לעזור לכם לבנות פרזנטציה באנגלית.

התהליך מחולק לארבעה שלבים:

1. פתיחת ההרצאה — בה תציגו את עצמכם ואת נושא הפרזנטציה, תמשכו את הקשב של הקהל ותיצרו קישור לחלק הבא.

2. גוף ההרצאה — בו תפרטו שני רעיונות מרכזיים בנושא שבחרתם.

3. סיכום ההרצאה — בו תחזרו על הרעיון המרכזי ותנסחו מסר בולט לקהל.

4. תרגול קולי — בו תוכלו להקליט הודעה ל־TedFriend ולקבל משוב בעל פה.

TedFriend ינחה אתכם מתי כדאי לעבור לשלב הבא, אבל אם תעדיפו תוכלו לעבור בין השלבים על ידי לחיצה על הכפתורים בצד ימין של השיחה.

שימו לב: הדברים שתכתבו כאן חשופים ל־TedFriend ולמדריכות התוכנית. הקפידו לא לשתף סיסמאות או מידע אישי רגיש.
```

### Rules

- Do not show the popup repeatedly in existing conversations.
- Do not change the learning flow.
- Do not change login/authentication logic unless explicitly approved.
- Keep copy gender-inclusive.
- Follow `copywriting.md`, `design.md`, and `accessibility.md`.

---

# General Implementation Rules for All Tasks

For every task:

1. Explain the plan before editing.
2. List the exact files expected to change.
3. Ask for approval before risky changes.
4. Make the smallest safe code change.
5. Do not combine unrelated tasks.
6. Run `dotnet restore` and `dotnet build` when relevant.
7. Report exactly what changed.
8. Report what the developer should manually check.
