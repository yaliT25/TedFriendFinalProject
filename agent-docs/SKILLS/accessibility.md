# Accessibility Skill — TED Friend

## 1. Purpose
This skill defines accessibility rules for UI changes in the TED Friend project.

The system is used by young students and instructors. Accessibility must be preserved in every interface change.

## 2. General Rule
Every UI change must remain accessible, keyboard-friendly, readable, and understandable.

Do not treat accessibility as a separate cleanup task. Accessibility must be considered during the change itself.

## 3. Semantic HTML
Use semantic elements whenever possible.

Rules:
- Use native `<button>` elements for clickable actions.
- Do not use clickable `<div>` or `<span>` elements when a button or link is appropriate.
- Use real form labels for inputs.
- Use headings in a logical order.
- Do not create fake interactive elements without keyboard support.

## 4. Keyboard Navigation
The interface must be usable with a keyboard.

Rules:
- Interactive elements must be reachable with Tab.
- The visible tab order should follow the visual and logical order of the screen.
- Do not trap focus unless it is inside an intentional modal dialog.
- Modals must allow focus to stay inside the modal while open and return focus logically when closed.

## 5. Focus States
Keyboard focus must be visible.

Rules:
- Preserve or add visible `:focus-visible` styles.
- Do not remove outlines without replacing them with an accessible focus indicator.
- Focus indicators must be clear against the background.

## 6. Images and Alt Text
Every meaningful image must include alternative text.

Rules:
- Add `alt` text to meaningful images.
- Use empty `alt=""` only for purely decorative images.
- If unsure whether an image is meaningful or decorative, ask the user.
- Mascot images should usually have alt text only if they communicate information or guidance.

## 7. Color and Contrast
Rules:
- Do not rely on color alone to communicate status, errors, or progress.
- Preserve readable contrast between text and background.
- Avoid placing text on visually busy backgrounds unless readability is clearly preserved.
- Use color according to `design.md`.

## 8. Forms and Errors
Rules:
- Inputs must have clear labels or accessible names.
- Error messages should be close to the relevant field.
- Error messages should be clear, calm, and non-technical.
- Do not expose technical API or database errors to students.

## 9. Student-Friendly Accessibility
Because the student audience is grades 4–6:
- keep instructions short;
- avoid dense text blocks;
- use clear buttons;
- avoid confusing icon-only actions unless labels or accessible names exist;
- avoid sudden layout shifts that may confuse young users.

## 10. Manual Accessibility Checks
After UI changes, check:
- Can all actions be reached with Tab?
- Is focus visible?
- Do buttons have clear text or accessible names?
- Do images have appropriate alt text?
- Are error states understandable?
- Is the layout readable on mobile, tablet, and desktop?
