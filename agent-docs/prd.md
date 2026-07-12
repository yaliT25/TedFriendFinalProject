# PRD — TED Friend (Classi)

## 1. Product Summary
TED Friend is a digital learning assistant for students in grades 4–6 who are building and practicing a short English TED-style presentation as part of the Classi program.

The product includes two connected parts:
- a student-facing chat-based assistant that guides the learner through the presentation-building process;
- an instructor dashboard that helps the instructor track student progress and identify students who need support.

TED Friend is a complementary learning tool. It does not replace the instructor, the Classi program, or classroom guidance.

## 2. Target Users

### Primary users — students
Students in grades 4–6 who are learning English as a foreign language and need support in planning, writing, and practicing a personal presentation.

### Secondary users — instructors
Classi instructors who manage groups of students and need a clear overview of progress, help requests, and student practice status.

## 3. Product Goals
The product should help students:
- choose and organize a personal topic;
- understand the structure of a TED-style presentation;
- build an opening, body, and conclusion;
- practice speaking in English;
- receive supportive feedback;
- feel more confident speaking in front of an audience.

The product should help instructors:
- monitor student progress;
- identify students who need help;
- view key learning indicators without reading every full conversation manually;
- support students more efficiently during or between sessions.

## 4. Core Student Flow
The student flow must remain clear, gradual, and age-appropriate.

Main phases:
1. Login / entry to the system.
2. Topic selection or topic input.
3. Opening — short self-introduction, hook, and transition sentence to the body.
4. Body — two main ideas or arguments that support the topic.
5. Conclusion — short recap of the main idea and a closing sentence with a main takeaway.
6. Voice practice / speaking practice.
7. Summary screen — feedback, tips, and self-report help status.

The agent must not change the learning flow, phase order, pedagogical structure, or student progression rules without explicit user approval.

## 5. Core Instructor Dashboard Flow
The instructor dashboard should provide a simple, readable overview of the class.

The dashboard may include:
- list of students;
- each student’s last phase;
- selected topic;
- practice or progress indicators;
- AI feedback scores when available;
- help request status;
- session count;
- class-level counters such as total students, total sessions, and help requests.

The instructor dashboard is an instructor-facing area only.
It must remain professional, clear, and useful for instructors, and must not be redesigned as a child-facing or visually childish interface.
The agent must not expose dashboard content to students or change dashboard access logic without explicit approval.
Dashboard UI or data-display changes are allowed only when explicitly requested by the developer, and must preserve the dashboard’s professional purpose.

## 6. Product Tone

### Student interface
The tone should be:
- friendly;
- encouraging;
- simple;
- calm;
- suitable for grades 4–6;
- supportive rather than corrective or judgmental.

### Instructor interface
The tone should be:
- professional;
- concise;
- clear;
- action-oriented.

## 7. Language Requirements
The system includes both English and Hebrew.

Rules:
- Student-facing English must be simple and age-appropriate.
- Hebrew interface copy must be gender-inclusive whenever it addresses the user.
- Do not change existing copywriting unless the user explicitly requested it.
- Do not mix Hebrew and English unnecessarily.
- Keep button labels short and clear.

## 8. Non-Goals
The agent must not add features that are outside the current project scope.

Do not add without explicit approval:
- a completely new presentation editor;
- a new dashboard module;
- a new authentication method;
- a new database structure;
- a new AI model or external AI provider;
- native mobile app behavior;
- major redesigns;
- new gamification systems;
- new analytics beyond the current dashboard needs.

## 9. Critical Product Boundaries
The following areas are sensitive and must not be changed without explicit approval:
- student learning flow;
- instructor dashboard logic;
- Google login/authentication;
- database schema;
- OpenAI / Whisper request structure;
- system prompts;
- UI/UX design direction;
- routing and API endpoint names;
- data stored or displayed to instructors.

## 10. Success Criteria
A change is successful only if:
- it solves the specific requested issue;
- it does not introduce unrelated changes;
- it preserves the existing learning flow;
- it remains responsive across desktop, tablet, and mobile;
- it does not break authentication, database operations, or AI communication;
- it keeps the system clear for both students and instructors;
- it follows the project rules, design skill, accessibility skill, and technical constraints.
- it solves the issue with the fewest reasonable lines of readable, safe code;
