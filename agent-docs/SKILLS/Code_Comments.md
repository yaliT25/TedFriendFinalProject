# Skill: Code Comments for Project Evaluation

## Purpose
This skill defines how the agent should add code comments in the Ted Friend final project.

The comments are intended for project evaluators, instructors, and reviewers - not for the project owner personally. Comments must explain the role of the code in a general, professional, and readable way.

## Main Rule
The agent should add comments only when they help a project evaluator understand the structure, purpose, or learning flow of the project.

The goal is not to document every line of code. The goal is to make the project easier to evaluate without overloading the code with unnecessary comments.

## Language Rule
All instructions in this skill are written in English.

Actual comments added inside project files must be written in Hebrew only.

This applies to comments in:
- HTML / Razor markup
- C# code inside Razor files
- Controller methods
- CSS files


## Commenting Style
Comments must be general, professional, and explanatory.

They should describe what the relevant section is responsible for, not what the agent changed and not what was discussed with the user.

Use comments that explain the role of the code, such as:

```html
<!-- רכיב זה מציג את נתוני ההתקדמות של התלמידים -->
```

```csharp
// שיטה זו שומרת את תשובות התלמיד לאחר סיום השיחה
```

```css
/* הגדרות עיצוב כלליות למסכי הדשבורד */
```

Do not write personal, conversational, or task-specific comments, such as:

```csharp
// כמו שביקשת, הוספתי בדיקה חדשה
// שינוי לפי השיחה שלנו
// תיקון זמני ליהלי
// כאן תיקנתי את הבאג
// הקוד הזה נכתב מחדש
```

Comments should explain what the code does, not why the agent changed it.

## Where Comments Should Be Added
The agent may add comments only in the following places.

### 1. HTML / Razor markup sections
Add comments only for meaningful page areas, components, or sections.

Good example:

```html
<!-- אזור זה מציג את סיכום ההתקדמות של התלמיד -->
```

### 2. C# code embedded inside HTML / Razor files
Add comments only when the logic affects the displayed interface, the user flow, or the learning process.

Good example:

```razor
@* בדיקה זו קובעת האם להציג הודעת שגיאה למשתמש *@
```

### 3. End of controller methods
At the end of each relevant controller method, add a short comment that summarizes the purpose or result of the method.

Good example:

```csharp
// סיום השיטה: החזרת נתוני התלמידים לדשבורד המדריכה
```

### 4. General CSS sections
Add comments only for broad styling areas that affect an entire page, layout, component group, or repeated interface pattern.

Good example:

```css
/* הגדרות עיצוב כלליות למסכי שיחה עם Ted Friend */
```

5. **DTO classes**

   Add comments to DTO classes when the comment helps evaluators understand what type of data the DTO represents and where that data is used in the system.

   The comment should describe the DTO’s general role in the project, especially when it transfers data between the database, controllers, AI flow, student interface, or instructor dashboard.

   Example:
   ```csharp
   // DTO זה מרכז את נתוני ההתקדמות של התלמיד לצורך הצגה בדשבורד המדריכה
   public class StudentProgressDto
   {
       public int ConversationId { get; set; }
       public string StudentName { get; set; }
       public string TopicTitle { get; set; }
   }

## Where Comments Should Not Be Added
The agent must avoid adding comments everywhere in the code.

Do not add comments:
- Above every variable.
- Above every small line of logic.
- Above obvious code.
- Inside every CSS rule.
- For every single UI element.
- Inside the system prompt.
- In comments that describe the agent's work process.
- In comments that mention the user, the conversation, or the specific change request.

## Level of Detail
Comments should stay at a high or medium level.

Good example:

```html
<!-- אזור העלאת הקובץ שבו התלמיד מוסיף מידע על נושא ההרצאה -->
```

Too detailed:

```html
<!-- כפתור זה מפעיל את הפונקציה שמעדכנת את selectedFile כאשר המשתמש בוחר קובץ -->
```

Good example:

```css
/* התאמות רספונסיביות למסכי מובייל */
```

Too detailed:

```css
/* שינוי מרווח ימני של 8 פיקסלים לכפתור השליחה */
```

## System Prompt Rule
The agent must not add comments to the system prompt.

If the agent edits or reviews a system prompt, it may explain its changes in the response to the user, but it must not insert code-style comments into the prompt itself.

## Before Adding a Comment
Before adding a comment, the agent must ask itself:

Would this comment help a project evaluator understand the system?

If the answer is no, the agent must not add the comment.
