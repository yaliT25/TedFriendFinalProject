# Skill: UI Design, Visual Consistency, and Responsiveness

## Purpose
This skill defines how the agent should handle UI design, visual styling, CSS, and responsive front-end implementation in the TED Friend project.

The project is in its final correction stage. The agent must preserve the existing UI/UX and visual identity unless the user explicitly requests a design change.

The goal is to make small, safe, responsive, and consistent styling changes that match the existing TED Friend design system.

---

## 1. Core Design Principle
The agent must not redesign the interface.

When working on UI or CSS, the agent must:
- preserve the current look and feel;
- follow the existing design language;
- make only the smallest visual change required to solve the requested issue;
- avoid changing layouts, colors, typography, spacing, or interaction patterns unless explicitly requested;
- ask the user before making any visual decision that is not directly required by the task.

The agent must treat every UI change as potentially sensitive because the interface is intended for both elementary school students and instructors.

---

## 2. Design System Consistency
The agent must follow the existing TED Friend design guidelines and avoid introducing a new visual language.

The design system includes:
- a soft, playful, cloud-based background and texture style;
- the TED Friend logo;
- a friendly companion mascot;
- rounded cards and containers;
- a child-friendly but organized color palette;
- clear and familiar icons;
- the Fredoka font for interface text;
- the Acme font only where it already appears as part of the logo identity.

The agent must not introduce new fonts, unrelated illustration styles, unrelated icon styles, or new color schemes without explicit approval.

---

## 3. Typography Rules
The project uses Fredoka as the main interface font.

When adding or updating UI text styles, the agent must:
- keep Fredoka as the main font for interface text;
- rely on the global font definition whenever possible;
- avoid redefining font families locally unless required by a specific bug fix;
- preserve the existing hierarchy of headings, subtitles, labels, and body text;
- use relative units such as `rem` for font sizes.

The agent must not use hardcoded visual overrides that break the project typography hierarchy.

If the user asks to match a Figma text size, convert the size to `rem` using the project root size as the reference. Do not paste pixel-based typography directly into CSS.

---

## 4. Color Palette and Visual Balance
The agent must preserve the existing TED Friend color palette and visual balance.

The design guide defines a young, friendly, and readable palette. The palette combines bright playful colors with calmer blue, green, and dark gray tones to keep the interface suitable for elementary school students while still looking organized and professional.

### Approved TED Friend Palette
Use existing CSS variables or existing project classes whenever they already define these colors. If a new color value is required, prefer the approved palette below instead of introducing a new color.

| Role | Color | Hex | Usage Guidance |
|---|---:|---:|---|
| Soft sky background | Light sky blue | `#BBE2F9` | Main cloud background, soft page atmosphere, large quiet areas. |
| Primary light blue | Blue | `#6CADE6` | Student-facing panels, friendly highlights, non-alert visual emphasis. |
| Positive / stable accent | Green | `#008542` | Progress, positive states, success, stability, reassuring status indicators. |
| Primary deep brand tone | Deep blue | `#28498B` | Structural emphasis, headings, selected states, dashboard emphasis. |
| Action / playful accent | Yellow | `#FFCD5A` | Main CTA buttons, friendly highlights, tips, playful attention points. |
| TED emphasis / alert accent | Red | `#E62B1E` | TED-related emphasis, recording/audio emphasis, important alerts, destructive actions only when appropriate. |
| Text / contrast | Dark gray | `#333333` | Main readable text, icons, contrast, neutral emphasis. |

### Color Usage Rules
When updating colors, the agent must:
- reuse existing project colors, CSS variables, or already-defined classes whenever possible;
- preserve the soft blue/cloud background identity;
- keep major containers visually calm and readable;
- use yellow mainly for primary action buttons, tips, or friendly attention areas;
- use red carefully, mainly for TED identity, recording/audio emphasis, alerts, or destructive actions;
- use green for progress, success, or reassuring status indicators;
- use dark gray for readable text and icon contrast;
- avoid adding new bright colors that increase visual noise;
- avoid changing brand colors without explicit user approval.

The interface should stay friendly, clear, and suitable for elementary school students without becoming visually overloaded.

If a needed color is not covered by the approved palette or existing codebase variables, the agent must ask the user before introducing it.

---

## 5. Responsive Layout Rules
The interface must remain responsive across desktop, tablet, and mobile screens.

The agent must build and fix layouts using:
- Flexbox;
- CSS Grid;
- responsive wrappers;
- relative spacing;
- media queries only when needed.

The agent must not rely on rigid screen-specific placement that works only on one resolution.

Before completing a UI change, the agent must check whether the change could break:
- desktop layout;
- tablet layout;
- mobile layout;
- Hebrew and English mixed-direction content;
- scrolling behavior;
- button clickability;
- mascot or logo placement.

---

## 6. Absolute Positioning Ban
Absolute positioning is forbidden for page layout unless the user explicitly approves it.

The agent must not use `position: absolute`, `top`, `left`, `right`, or `bottom` to place main layout elements, cards, logos, side panels, chat areas, dashboard sections, or the companion mascot.

Use Flexbox or Grid instead.

Absolute positioning may be used only when all of the following are true:
- it is required for a small decorative or overlay element;
- it does not control the main page structure;
- it does not break responsiveness;
- it does not block clicks or focus;
- the reason is explained clearly to the user before implementation, unless the user explicitly requested it.

The agent must never copy rigid absolute coordinates from Figma into the project without adapting them into a responsive layout.

---

## 7. REM-Based Sizing and Spacing
The agent must use `rem` units for sizes and spacing whenever possible.

Use `rem` for:
- width and max-width;
- height and min-height when needed;
- margin;
- padding;
- gap;
- border-radius;
- font-size;
- layout spacing.

Avoid hardcoded `px` values for layout, sizing, and spacing.

Permitted exceptions:
- `box-shadow` values copied from the existing visual style;
- very thin borders such as `1px` when visually required;
- technical values that are not layout-defining and cannot reasonably be expressed in `rem`;
- values that already exist in the project and should not be refactored as part of a small fix.

If a Figma value is provided in pixels, convert it to `rem` instead of pasting the pixel value directly.

---

## 8. No Inline Styles
The agent must not write inline styles inside `.razor` or HTML markup.

Do not write:

```html
<div style="margin-top: 20px; color: red;">
```

All styling must be placed in the relevant isolated CSS file, such as:

```text
ComponentName.razor.css
```

Use semantic class names that describe the role of the element rather than its temporary visual state.

The agent may use dynamic classes in Razor when the UI state changes, but the visual definitions must still live in CSS.

---

## 9. Isolated CSS and Scope Control
The agent must keep component styling scoped and organized.

When editing styles, the agent must:
- prefer the component's `.razor.css` file;
- avoid changing global CSS unless the change truly affects the whole application;
- avoid creating broad selectors that unintentionally affect unrelated screens;
- avoid refactoring CSS files that are not part of the requested fix;
- add only high-level CSS comments when they help evaluators understand a section.

Global CSS changes require extra caution and should be explained before implementation.

---

## 10. Mascot, Logo, and Visual Assets
The TED Friend logo and companion mascot are part of the project identity.

When placing or adjusting visual assets, the agent must:
- preserve aspect ratio using `height: auto`, `width: auto`, `max-width`, or `object-fit: contain` as appropriate;
- avoid stretching, cropping, or distorting logos and character images;
- keep the mascot aligned through layout containers rather than fixed screen coordinates;
- ensure the mascot does not cover buttons, text, inputs, or scrollable content;
- preserve clickability of all interactive elements around layered assets;
- include meaningful `alt` text for images unless the image is purely decorative.

Do not replace the logo, mascot, icons, or textures without explicit user approval.

---

## 11. Buttons, Icons, and Interactive Elements
Buttons and icons must remain clear, familiar, and accessible for young learners.

The agent must:
- use native `<button>` elements for clickable actions whenever possible;
- avoid attaching click handlers to non-interactive layout elements unless there is a justified accessibility reason;
- preserve visible hover and focus states;
- keep icons visually consistent with the existing icon style;
- avoid adding icons that are not part of the existing visual language;
- ensure that important buttons remain easy to identify and click on all screen sizes.

Any new icon must include an accessible label, visible text, or appropriate `aria-label` when needed.

---

## 12. Modal and Overlay Rules
Modals, popups, and overlays must not break the page underneath them.

When editing or creating a modal, the agent must:
- use a clear backdrop layer;
- ensure the modal appears above the page content;
- prevent accidental clicks on elements behind the modal;
- keep keyboard navigation usable;
- keep close, confirm, and cancel actions visible and accessible;
- avoid using absolute positioning for the entire modal layout unless it is part of an existing approved modal pattern.

The agent must not create a modal that traps the user visually or functionally without a clear exit action.

---

## 13. Accessibility and Keyboard Use
Every UI change must preserve accessibility.

The agent must:
- keep semantic HTML structure;
- use native form controls and buttons when possible;
- preserve keyboard navigation;
- ensure visible focus states using `:focus-visible` or an existing focus style;
- ensure images have `alt` text when they convey meaning;
- avoid relying only on color to communicate meaning;
- maintain readable contrast;
- preserve logical reading order in mixed Hebrew-English screens.

If an accessibility requirement conflicts with a visual detail, the agent must ask the user before choosing a compromise.

---

## 14. Hebrew-English Interface Direction
The project includes both Hebrew and English content.

The agent must be careful when editing layout or markup that includes mixed languages.

The agent must:
- preserve existing `dir`, `lang`, and alignment decisions;
- avoid forcing all content into one direction when the screen intentionally includes both Hebrew and English;
- check that punctuation, numbers, and English presentation text remain readable inside Hebrew UI screens;
- avoid CSS changes that visually reverse or misalign mixed-language content.

---

## 15. Figma-to-Code Rule
Figma should guide the design, but it must not be copied into code blindly.

When translating Figma values into implementation, the agent must:
- convert fixed pixel sizes into responsive `rem` units;
- replace absolute coordinates with Flexbox or Grid layout;
- preserve proportions rather than exact static placement;
- avoid copying one-screen-only dimensions that break on other devices;
- ask the user before making design decisions that are not explicit in the task.

The project must work as a responsive web application, not as a static Figma frame.

---

## 16. Change Scope and Approval
The agent must not make broad design changes without approval.

The agent must ask before:
- changing the overall layout of a screen;
- changing color palette decisions;
- changing typography hierarchy;
- moving the mascot, logo, dashboard panels, or chat structure in a meaningful way;
- introducing global CSS changes;
- replacing icons or visual assets;
- adding new animations;
- changing responsive breakpoints across multiple screens.

Small bug fixes are allowed when they directly solve the user-requested issue and follow this skill.

---

## 17. Final UI Check Before Reporting Completion
Before reporting that a UI/CSS task is complete, the agent must check:
- the change uses the smallest safe styling update;
- no inline styles were added;
- no unauthorized absolute positioning was added;
- layout units are relative whenever possible;
- the change is scoped to the relevant component CSS;
- desktop, tablet, and mobile behavior were considered;
- the visual language still matches TED Friend;
- accessibility was preserved;
- no unrelated UI/UX changes were made.

The final report to the user should mention only the files changed, the purpose of the change, and any approval needed for remaining design decisions.
