using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite; // לחיבור ל-SQLite
using Dapper;               // לעבודה עם שאילתות SQL
using BlazorGoogleLogin.Shared.DTOs;
using BlazorGoogleLogin.Server.Models;
using UglyToad.PdfPig;      
using NPOI.XWPF.UserModel;  
using System.Text;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorGoogleLogin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GPTController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly string _model;
        private readonly string _apiKey;
        private readonly string _connectionString;

        public GPTController(IConfiguration config)
        {
            _client = new HttpClient();
            _model = config.GetValue<string>("OpenAI:Model");
            _apiKey = config.GetValue<string>("OpenAI:Key");
            _connectionString = config.GetConnectionString("DefaultConnection");
            
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        #region ניהול סשנים (Dapper)

        [HttpPost("CreateSession")]
        public async Task<IActionResult> CreateSession([FromBody] int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            
            // שאילתת SQL להכנסת סשן חדש והחזרת ה-ID שנוצר
            string sql = @"INSERT INTO ChatSessions (UserId, Topic, ChatHistory, CreatedAt, CurrentPhase) 
                           VALUES (@UserId, 'New English Lesson...', '[]', @CreatedAt, 1);
                           SELECT last_insert_rowid();";

            var newId = await connection.ExecuteScalarAsync<int>(sql, new { 
                UserId = userId, 
                CreatedAt = DateTime.Now 
            });

            return Ok(newId);
        }

        [HttpGet("GetSessions/{userId}")]
        public async Task<IActionResult> GetSessions(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "SELECT Id, Topic, CreatedAt, CurrentPhase FROM ChatSessions WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            
            var sessions = await connection.QueryAsync<ChatSession>(sql, new { UserId = userId });
            return Ok(sessions);
        }
        
        [HttpPost("UpdatePhase/{sessionId}/{newPhase}")]
        public async Task<IActionResult> UpdatePhase(int sessionId, int newPhase)
        {
            using var connection = new SqliteConnection(_connectionString);
    
            // עדכון של השלב וזמן הפעילות האחרון
            string sql = "UPDATE ChatSessions SET CurrentPhase = @phase, LastActiveAt = @lastActive WHERE Id = @id";
    
            await connection.ExecuteAsync(sql, new { 
                phase = newPhase, 
                lastActive = DateTime.Now, 
                id = sessionId 
            });

            return Ok(new { success = true });
        }
        
        [HttpGet("GetSessionForChat/{sessionId}")]
        public async Task<IActionResult> GetSession(int sessionId)
        {
            using var connection = new SqliteConnection(_connectionString);
            // שליפה ממוקדת של השדות הקריטיים
            string sql = "SELECT Id, Topic, CurrentPhase FROM ChatSessions WHERE Id = @Id";
    
            var session = await connection.QueryFirstOrDefaultAsync<ChatSession>(sql, new { Id = sessionId });
    
            if (session == null) return NotFound();
            return Ok(session);
        }
        
        //שיטה לשמירת ההתקדמות
        [HttpPost("SaveFullProgress/{sessionId}")]
        public async Task<IActionResult> SaveFullProgress(int sessionId, [FromBody] SaveProgressRequest request)
        {
            using var connection = new SqliteConnection(_connectionString);
            string sql = "UPDATE ChatSessions SET ChatHistory = @History, CurrentPhase = @Phase WHERE Id = @Id";

            await connection.ExecuteAsync(sql, new { 
                History = request.History, 
                Phase = request.CurrentPhase,
                Id = sessionId 
            });

            return Ok();
        }

        //שיטה למחיקה של סשן שיחה עם הAI
        [HttpDelete("DeleteSession/{sessionId}")]
        public async Task<IActionResult> DeleteSession(int sessionId)
        {
            try 
            {
                using var connection = new SqliteConnection(_connectionString);
        
                // שאילתת המחיקה
                string sql = "DELETE FROM ChatSessions WHERE Id = @Id";
        
                // ExecuteAsync מחזירה את מספר השורות שהושפעו מהפעולה
                int rowsAffected = await connection.ExecuteAsync(sql, new { Id = sessionId });

                if (rowsAffected == 0)
                {
                    // אם לא נמחקה אף שורה, כנראה שה-ID לא קיים
                    return NotFound(new { success = false, message = "המפגש לא נמצא במערכת" });
                }

                return Ok(new { success = true, message = "המפגש נמחק בהצלחה" });
            }
            catch (Exception ex)
            {
                // שימוש ב-try-catch למקרה של תקלה בגישה ל-DB
                return StatusCode(500, $"שגיאה במחיקת המפגש: {ex.Message}");
            }
        }
        
        #endregion

        #region עיבוד קבצים וצ'אט

        // מסלול 1: עיבוד קובץ (PDF או Word)
        [HttpPost("UploadDocument/{sessionId}")]
        public async Task<IActionResult> UploadDocument(int sessionId, [FromForm] FileUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("לא הועלה קובץ.");

            try
            {
                // חילוץ הטקסט
                string extractedText = ExtractTextLogic(request);

                // יצירת כותרת AI מהטקסט שחולץ מהקובץ
                string aiTopic = await GenerateTopicWithAI(extractedText);
        
                // עדכון ה-Topic בבסיס הנתונים
                using var connection = new SqliteConnection(_connectionString);
                string sql = "UPDATE ChatSessions SET Topic = @Topic WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Topic = aiTopic, Id = sessionId });

                return Ok(extractedText);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בעיבוד הקובץ: {ex.Message}");
            }
        }

// מסלול 2: עיבוד קלט ידני (Text)
        [HttpPost("ProcessManualContext/{sessionId}")]
        public async Task<IActionResult> ProcessManualContext(int sessionId, [FromBody] string manualText)
        {
            if (string.IsNullOrWhiteSpace(manualText))
                return BadRequest("לא הוזן טקסט.");

            try 
            {
                // יצירת כותרת AI מהטקסט שהוקלד ידנית
                string aiTopic = await GenerateTopicWithAI(manualText);
        
                using var connection = new SqliteConnection(_connectionString);
                string sql = "UPDATE ChatSessions SET Topic = @Topic WHERE Id = @Id";
                await connection.ExecuteAsync(sql, new { Topic = aiTopic, Id = sessionId });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בעיבוד הקלט הידני: {ex.Message}");
            }
        }
        
        //פנייה לשיחה עם הAI
        [HttpPost("Chat/{sessionId}")]
        public async Task<IActionResult> Chat(int sessionId, [FromBody] ChatRequest request)
        {
            using var connection = new SqliteConnection(_connectionString);

            // שליפה יעילה של העמודות הרלוונטיות
            string sql = "SELECT CurrentPhase, Topic FROM ChatSessions WHERE Id = @Id";
            var sessionData = await connection.QueryFirstOrDefaultAsync(sql, new { Id = sessionId });

            if (sessionData == null) return NotFound("Session not found");

            // העברת הנתונים מה-DB לפונקציית העזר
            string assistantReply = await CallOpenAI(request, (int)sessionData.CurrentPhase, (string)sessionData.Topic);

            // עדכון ושמירת היסטוריית השיחה
            string historyJson = JsonSerializer.Serialize(request.History);
            string sqlUpdate = "UPDATE ChatSessions SET ChatHistory = @History WHERE Id = @Id";
            await connection.ExecuteAsync(sqlUpdate, new { History = historyJson, Id = sessionId });

            return Ok(assistantReply);
        }
        
        //תרגום התשובה של הAI
        [HttpPost("Translate")]
        public async Task<IActionResult> Translate([FromBody] JsonElement body)
        {
            try
            {
                // חילוץ הטקסט מה-JSON שנשלח מה-Blazor
                string text = body.GetProperty("text").GetString();

                if (string.IsNullOrWhiteSpace(text))
                    return BadRequest("No text provided.");

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { 
                            role = "system", 
                            content = @"
You are a translation assistant for a digital English-speaking coach.

Translate the following English text into Hebrew.
Keep the translation natural, clear, and suitable for Israeli children in grades 4-6.

Stay faithful to the original meaning, tone, and structure.
Do not add, remove, or rewrite ideas.
Keep questions, instructions, examples, button names, phase names, and UI labels as close as possible to the original.

Use gender-inclusive Hebrew when addressing the child.
Use slash forms ONLY when the masculine and feminine forms are written differently in unvocalized Hebrew.
Use slash forms only for direct gendered verbs or pronouns, such as: את/ה, יכול/ה, צריך/ה, נסה/י, מוכן/ה, בחר/י, כתוב/י.
Do NOT use slash forms when the word is spelled the same in masculine and feminine without nikud.
For example, write: שמך, שלך, אותך, לך.
Do NOT write: שמך/שמך, שלך/שלך, אותך/אותך, לך/לך.
Do not use masculine-only or feminine-only phrasing.
Do not avoid gender by using unnatural impersonal phrasing.
Do not invent new slash forms unless needed.
Keep the translation natural, clear, and suitable for children.

Return ONLY the Hebrew translation.

""Thought process: First, evaluate the student in English. Then, translate your insights into Hebrew. Finally, verify that no Arabic words 'leaked' into the Hebrew text before generating the final JSON.""
"
                        },
                        new { role = "user", content = text }
                    },
                    temperature = 0.25 // טמפרטורה נמוכה לתרגום מדויק יותר
                };

                var response = await _client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
        
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "Error from OpenAI API");

                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                string translatedText = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return Ok(new { translation = translatedText.Trim() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאה בתרגום: {ex.Message}");
            }
        }

        #endregion
        
        #region Session Summary & Feedback
        
        [HttpGet("GetFeedback/{sessionId}")]
        public async Task<IActionResult> GetFeedback(int sessionId)
        {
            using var connection = new SqliteConnection(_connectionString);
    
            // שליפת מידע רלוונטי לעמוד הסיכום
            string sql = @"SELECT FluencyScore, GrammarScore, AiAnalysisJson, Topic 
                   FROM ChatSessions 
                   WHERE Id = @Id";
    
            var result = await connection.QueryFirstOrDefaultAsync<ChatSessionDTO>(sql, new { Id = sessionId });

            if (result == null) return NotFound();
            return Ok(result);
        }
        
[HttpPost("GenerateFinalSummary/{sessionId}")]
public async Task<IActionResult> GenerateFinalSummary(int sessionId)
{
    try 
    {
        // 1. שליפת היסטוריית השיחה מה-DB (לפי ה-SessionId)
        using var connection = new SqliteConnection(_connectionString);
        var chatHistory = await connection.QueryAsync<string>(
            "SELECT ChatHistory FROM ChatSessions WHERE Id = @Id", new { Id = sessionId });
        
        string fullHistory = string.Join("\n", chatHistory);
        
        // בדיקה האם ההיסטוריה ריקה או מכילה רק את סימון המערך הריק מה-DB
        if (string.IsNullOrWhiteSpace(fullHistory) || fullHistory.Trim() == "[]")
        {
            // אם אין שיחה, אנחנו לא מעדכנים כלום ופשוט מחזירים הצלחה ריקה
            return Ok(new { success = true, message = "No history to analyze" });
        }

        // 2. קריאה ל-AI (הפונקציה הפרטית למטה)
        var aiResult = await CallOpenAIForAnalysis(fullHistory);

        // 3. אריזת העצות ל-JSON עבור העמודה AiAnalysisJson
        string jsonAdvice = JsonSerializer.Serialize(new { 
            Keep = aiResult.Keep, 
            Improve = aiResult.Improve 
        });

        // 4. עדכון העמודות בטבלה
        string sql = @"UPDATE ChatSessions SET 
                       FluencyScore = @Fluency, 
                       GrammarScore = @Grammar, 
                       AiAnalysisJson = @Json 
                       WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { 
            Fluency = aiResult.FluencyScore, 
            Grammar = aiResult.GrammarScore, 
            Json = jsonAdvice, 
            Id = sessionId 
        });

        return Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"שגיאה בניתוח המסכם: {ex.Message}");
    }
}

[HttpPost("SaveStudentFeedback/{sessionId}")]
public async Task<IActionResult> SaveStudentFeedback(int sessionId, [FromBody] StudentFeedbackDTO feedback)
{
    if (feedback == null) return BadRequest("No feedback provided.");

    try 
    {
        using var connection = new SqliteConnection(_connectionString);
        
        // עדכון העמודות הספציפיות שהתלמיד מילא + שינוי השלב בו הוא נמצא
        string sql = @"UPDATE ChatSessions SET 
                       NeedsHelpScore = @Score, 
                       FreeTextComment = @Comment,
                       CurrentPhase = 7 
                       WHERE Id = @Id";

        await connection.ExecuteAsync(sql, new { 
            Score = feedback.Rating, 
            Comment = feedback.Comment, 
            Id = sessionId 
        });

        return Ok(new { success = true });
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"שגיאה בשמירת משוב תלמיד: {ex.Message}");
    }
}

// פונקציית עזר פרטית בתוך הקונטרולר לדיבור עם OpenAI
        private async Task<AiAnalysisResult> CallOpenAIForAnalysis(string history)
        {
            var requestBody = new
            {
                model = _model, 
                messages = new[]
                {
                    new { 
                        role = "system",
                        content = @"
You are a supportive English teacher for children in grades 4-6.
Analyze the chat history between the student and the AI coach.

Evaluate only the student's English performance.

Return a JSON object with these EXACT fields:
1. FluencyScore: An integer between 5 and 10.
2. GrammarScore: An integer between 5 and 10.
3. Keep: A short, encouraging sentence in HEBREW about something specific the student did well.
4. Improve: A short, gentle sentence in HEBREW about one specific thing the student should practice.

Scoring rules:
- The scores MUST be in the range of 5 to 10.
- 5 = needs much practice.
- 6-7 = good basic level.
- 8-9 = strong level.
- 10 = excellent for this age and context.
- Be supportive, but honest.
- Do not give a score lower than 5.
- FluencyScore and GrammarScore are evaluated separately.
- They do NOT need to be the same score.
- A student can have strong grammar but lower fluency, or strong fluency but lower grammar.
- GrammarScore is based on written English accuracy.
- FluencyScore is based on speaking flow, clarity, pace, and confidence.
- Do not copy the same score for both fields unless the chat clearly supports it.

Feedback structure:
- Keep MUST follow this structure:
  ""כל הכבוד שהשתמשת ב[specific skill or behavior] — זה עזר ל[positive result].""

- Improve MUST follow this structure:
  ""בפעם הבאה, נסה/י לעבוד על [specific skill or behavior] כדי ש[positive result].""

Feedback rules:
- Keep and Improve MUST be different.
- Keep and Improve MUST be specific to the chat content.
- The feedback must be based on the student's actual chat, not generic advice.
- Do NOT write vague feedback.
- Do NOT write feedback like:
  ""תקפיד/י על האותיות""
  ""נסה/י לכתוב משפטים שלמים""
  ""תמשיך/י להשתפר""
  ""עבודה טובה""
  ""כל הכבוד""
- Do NOT use broad categories alone, such as ""grammar"", ""pronunciation"", ""letters"", or ""sentences"".
- If you mention pronunciation, specify what exactly to practice:
  a sound, a word, word ending, speaking pace, pauses, or clarity.
- If you mention grammar, specify what exactly to practice:
  word order, verb tense, subject + verb, missing words, prepositions, sentence structure, or connectors.
- If there is no clear specific mistake, give one concrete next-step tip instead.
- Every Improve sentence must include a specific skill, behavior, or short English example.

Good Keep examples:
- ""כל הכבוד שהשתמשת במשפטים קצרים וברורים — זה עזר להבין את הרעיון המרכזי.""
- ""כל הכבוד שהוספת דוגמה לרעיון שלך — זה עזר להרצאה להישמע מעניינת יותר.""
- ""כל הכבוד שהשתמשת במילים פשוטות באנגלית — זה עזר למסר שלך להיות ברור.""

Good Improve examples:
- ""בפעם הבאה, נסה/י לעבוד על סדר המילים במשפט כדי שהרעיונות יהיו ברורים יותר.""
- ""בפעם הבאה, נסה/י לעבוד על חיבור בין רעיונות עם because או also כדי שההרצאה תזרום יותר.""
- ""בפעם הבאה, נסה/י לעבוד על דיבור איטי יותר כדי שיהיה קל יותר להבין אותך.""
- ""בפעם הבאה, נסה/י לעבוד על עצירה קצרה אחרי כל רעיון כדי שהדיבור יהיה ברור יותר.""
- ""בפעם הבאה, נסה/י לעבוד על הצליל th במילים כמו think כדי שההגייה תהיה ברורה יותר.""

Hebrew language rules:
- Write the Hebrew feedback in simple, clear Hebrew suitable for children.
- Use gender-inclusive Hebrew when addressing the student.
- Use ONLY these slash forms:
  את/ה, יכול/ה, צריך/ה, רוצה/ה, מוכן/ה, נסה/י, בחר/י, כתוב/י, המשך/י, תרגל/י.
- Do NOT use forms like נסי/ה, כתבי/ה, בחרי/ה.
- Do NOT use masculine-only or feminine-only phrasing.
- Do NOT use feminine-only phrasing.

CRITICAL LANGUAGE RULES:
- Strictly FORBIDDEN to use any Arabic characters or words (e.g., avoid ""أثناء"", ""كل"").
- The output must be 100% Hebrew script only.
- If you are unsure of a word in Hebrew, use a simpler Hebrew synonym.
- Before returning the JSON, double-check that every character is a valid Hebrew letter or a standard punctuation mark.

Length rules:
- Keep must be one sentence only, up to 20 Hebrew words.
- Improve must be one sentence only, up to 24 Hebrew words.

Return ONLY valid JSON.
Do not add explanations, markdown, or text outside the JSON.
"
                    },
                    new { 
                        role = "user", 
                        content = $"Analyze this conversation: {history}" 
                    }
                },
                response_format = new { type = "json_object" }
            };
    
            var response = await _client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var jsonContent = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return JsonSerializer.Deserialize<AiAnalysisResult>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

       
        #endregion
        
       #region פונקציות עזר פנימיות (Helper Methods)

// לוגיקה לחילוץ טקסט מקבצים - תומכת ב-PDF וב-Word
private string ExtractTextLogic(FileUploadRequest request)
{
    string extension = Path.GetExtension(request.File.FileName).ToLower();
    StringBuilder sb = new StringBuilder();

    using (var stream = request.File.OpenReadStream())
    {
        if (extension == ".pdf")
        {
            using (var pdf = PdfDocument.Open(stream))
            {
                foreach (var page in pdf.GetPages()) sb.Append(page.Text);
            }
        }
        else if (extension == ".docx")
        {
            var doc = new XWPFDocument(stream);
            foreach (var para in doc.Paragraphs) sb.AppendLine(para.Text);
        }
    }
    return sb.ToString();
}

// פנייה ל-AI כדי ליצור כותרת קצרה באופן אוטומטי מהטקסט שחולץ
private async Task<string> GenerateTopicWithAI(string text)
{
    // לוקחים רק את ההתחלה כדי לחסוך בזמן ובעלות
    string snippet = text.Length > 800 ? text.Substring(0, 800) : text;

    var prompt = new
    {
        model = _model,
        messages = new[] {
            new { role = "system", content = "You are a helpful assistant. Create a 3-4 word title in English for the provided text. Return ONLY the title." },
            new { role = "user", content = snippet }
        }
    };

    var response = await _client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", prompt);
    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
    
    // חילוץ המחרוזת מתוך מבנה ה-JSON של OpenAI
    string title = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
    return title.Trim('\"', '.', ' '); // ניקוי תווים מיותרים
}

// ניהול הפנייה המרכזית ל-GPT עבור שיחת הצ'אט הפדגוגית
        private async Task<string> CallOpenAI(ChatRequest request, int currentPhase, string topic)
{
    // הגדרת הנחיות מערכת (System Instructions) בהתאם לשלב הנוכחי בתהליך הלמידה
   // 1. Phase-specific instructions
string phaseInstructions = currentPhase switch
    
{
    2 => @"
PHASE 2: THE OPENING / HOOK.

Goal: By the end of this phase, the child should have:
1. One short self-introduction sentence that includes the child's name.
2. One short hook that opens the talk and catches attention.
3. One short transition sentence that connects the opening to the body.

The self-introduction MUST include:
- the child's name

The self-introduction MAY also include:
- age
- where the child is from
- the talk topic
- why the topic is important to them

The hook can be:
- a question
- a surprising fact
- a very short personal story

Instructions:
- First, help the child create a short self-introduction sentence.
- Make sure the self-introduction includes the child's name.
- Do not force the child to include age, place, topic, or personal reason unless they want to.
- Do not ask only ""What is your name?"" as the full task.
- Do not force all students to use the exact same self-introduction sentence.
- Offer 1-2 simple self-introduction examples, then help the child write their own.
- Then help the child create one hook.
- After the hook, help write one short transition sentence to the body of the talk.
- Keep the opening short and clear.
- Do NOT write the body.
- Do NOT write the full talk yet.",

    3 => @"
PHASE 3: THE BODY.
Goal: By the end of this phase, the child should have:
1. First main idea.
2. One short explanation or example for the first idea.
3. Second main idea.
4. One short explanation or example for the second idea.

For each idea:
- Write one simple main sentence.
- Add one short explanation or example.
- Keep sentences short and clear.
- Do NOT create a conclusion yet.",

    4 => @"
PHASE 4: THE CONCLUSION.
Goal:  By the end of this phase, the child should have:
1. a short recap of the main idea
2. one simple takeaway message

Instructions:
- Keep it short and memorable.
- Use simple English.
- Do NOT add new arguments or facts.",

    5 => @"
PHASE 5: VOICE PRACTICE.
Goal:
By the end of this phase, the child should:
1. Practice saying the talk out loud.
2. Get short feedback on speaking clearly.
3. Get feedback only on major pronunciation problems.
4. Know where to pause and make eye contact in the real presentation.

Instructions:
- Act like a kind speaking coach.
- Give short, encouraging feedback.
- Focus only on major pronunciation mistakes.
- Do not correct every small grammar mistake.
- Encourage the child to speak slowly and clearly.
- Remind the child that in the real presentation, they should make eye contact:
  1. after the hook
  2. after each main idea
  3. after the conclusion
- Do NOT give final grades yet.",

    6 => @"
PHASE 6: FINAL SUMMARY AND FEEDBACK.
This phase is NOT part of the talk.
Goal: Give the child a kind final evaluation.

Provide:
1. Grammar score from 1 to 10, based on phases 2-4.
2. Fluency score from 1 to 10, based on phase 5.
3. One thing the child did well.
4. One personal and practical tip for the real presentation.

Rules:
- Scores should usually be 5 or higher.
- Be honest but supportive.
- Keep the feedback short and encouraging.",

    _ => @"
GENERAL GUIDANCE.
Help the child improve their English TED-style talk in a supportive and age-appropriate way."
};


string nextButtonName = currentPhase switch
{
    2 => "גוף ההרצאה",
    3 => "סיכום",
    4 => "תרגול קולי",
    5 => "סיכום ומשוב",
    _ => ""
};

string phaseEndingInstruction = !string.IsNullOrEmpty(nextButtonName)
    ? $@"
Phase ending behavior:
- When the child completes the goal of the current phase, give short, warm feedback.
- Invite the child to continue by pressing the button ""{nextButtonName}"".
- Keep the transition message natural, friendly, and short.
- Do NOT start the next phase yourself.
"
    : "";

// 2. System message
string systemRole = $@"
You are a supportive English teacher for Israeli children in grades 4-6.

Audience:
- The child is a non-native English speaker.
- Their first language is Hebrew.
- They may make common mistakes with word order, tenses, prepositions, and sentence structure.

Language:
- Always reply in English, even if the child writes in Hebrew.
- The learning conversation must stay in English.
- Do not answer in Hebrew.
- Use simple, short, clear English.
- Use vocabulary suitable for grades 4-6.
- Do not use high-level or academic English.

Pedagogical rules:
- Be warm, patient, and encouraging.
- Do not overwhelm the child.
- Keep every response short: usually 2-3 short sentences maximum.
- Ask one question at a time.
- Ask only one question at the end of the response.
- Prefer guiding the child instead of writing everything for them immediately.
- Give examples, then invite the child to choose, change, or continue.
- Correct only mistakes that hurt understanding.
- When correcting, be gentle and show the better version.

Project context:
- The child is creating a short TED-style talk.
- The talk should be up to 5 minutes.
- The talk itself includes only: opening, body, and conclusion.
- Voice practice is a separate practice phase in the AI conversation. It is not part of the talk structure.

Current phase instructions:
{phaseInstructions}
Stay strictly within this phase. Do not move to other parts of the talk.

End of phase instruction:
{phaseEndingInstruction}
IMPORTANT: The only way to move to the next phase is for the user to click the button. 
Do not provide content for the next phase yourself.

The Topic of the Ted Talk:
{topic}

Source material / previous work:
{request.DocumentContent}
";

// 3. Build messages for OpenAI
var messagesForOpenAI = new List<object>
{
    new
    {
        role = "system",
        content = systemRole
    }
};


// 4. Add topic as the first user message
if (!string.IsNullOrEmpty(topic))
{
    messagesForOpenAI.Add(new
    {
        role = "user",
        content = $@"
Hi! My TED Talk topic is: {topic}.
Please help me with Phase {currentPhase}.
"
    });
}

    // הוספת היסטוריית השיחה הקיימת לבקשה לצורך שמירה על הקשר (Context)
    if (request.History != null)
    {
        foreach (var msg in request.History)
        {
            messagesForOpenAI.Add(new { role = msg.role, content = msg.content });
        }
    }

    // הגדרת אובייקט הבקשה עבור OpenAI API
    var gptRequest = new
    {
        model = _model,
        messages = messagesForOpenAI,
        temperature = 0.7
    };

    // ביצוע קריאת ה-POST ל-API וקבלת המענה
    var response = await _client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", gptRequest);
    
    // ניתוח (Parsing) של תשובת ה-JSON לחילוץ תוכן ההודעה בלבד
    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
    
    // גישה למבנה הנתונים: choices -> index 0 -> message -> content
    string assistantReply = jsonResponse.GetProperty("choices")[0]
                                        .GetProperty("message")
                                        .GetProperty("content")
                                        .GetString();

    return assistantReply;
}

#endregion
    }
}