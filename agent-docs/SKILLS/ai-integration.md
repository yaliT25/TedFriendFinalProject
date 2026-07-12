# AI Integration Skill — TED Friend

## 1. Purpose
This skill defines how the agent must handle OpenAI, Whisper, prompts, and AI-related behavior in the TED Friend project.

AI behavior is pedagogically sensitive. The agent must not change it casually.

## 2. System Prompt Safety
Rules:
- Do not change system prompts without explicit user approval.
- Do not rewrite prompts for style, elegance, or optimization unless requested.
- Do not expose system prompts in the UI, logs, browser console, or API responses.
- Do not add comments inside system prompts.
- If a requested change affects prompt behavior, explain the expected effect before implementing.

## 3. AI Flow Preservation
The AI supports a structured learning flow.

Do not change without approval:
- phase order;
- step gating;
- opening/body/conclusion structure;
- voice practice behavior;
- scoring behavior;
- feedback style;
- student language level;
- dashboard data derived from AI interactions.

## 4. Model and API Changes
Do not change without explicit approval:
- model name;
- endpoint;
- request payload structure;
- response parsing;
- temperature or generation parameters;
- storage behavior;
- API provider;
- Whisper/audio handling behavior.

## 5. Failure Handling
AI failures must be handled gracefully.

Rules:
- Never crash the UI because of an AI or Whisper error.
- Never leave the UI in an infinite loading state.
- Do not show raw technical errors to students.
- Update the UI state clearly when a request fails.
- Allow the user to try again when appropriate.

Approved student-facing fallback message:
`Ted Friend לא יכול לענות כרגע, נסו שוב מאוחר יותר.`

## 6. Privacy and Data Safety
Rules:
- Do not store new personal data unless explicitly requested.
- Do not send unnecessary personal data to AI services.
- Do not expose student conversations to other students.
- Dashboard data must remain limited to the instructor view.
- Preserve existing Google authentication and user ownership logic.

## 7. Student Feedback Style
AI feedback must stay:
- supportive;
- short;
- simple;
- suitable for grades 4–6;
- focused on helping the student continue;
- not overly critical or discouraging.

## 8. When to Ask Before Implementing
Ask for approval before any change that affects:
- prompt content;
- AI response logic;
- scoring;
- phase progression;
- speech practice;
- stored AI data;
- displayed AI feedback;
- instructor dashboard AI summaries.

## 9. Main Rule
AI changes are not regular code cleanup.

Every AI-related change must preserve the educational purpose, student safety, and project scope.
