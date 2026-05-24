namespace BlazorGoogleLogin.Shared.DTOs;

public class ChatSession
{
    // מפתח ראשי - מזהה ייחודי לכל שיחה
    public int Id { get; set; }

    // קישור למשתמש (Forepublicign Key)
    public int UserId { get; set; }

    // נושא השיחה (יעודכן אוטומטית על ידי ה-AI)
    public string Topic { get; set; }

    // השלב הנוכחי בתהליך הלמידה (למשל: 1=קריאה, 2=צ'אט, 3=סיכום)
    public int? CurrentPhase { get; set; }

    // מדדי הערכה פדגוגיים (הציונים שהכנת בטבלה)
    public int? FluencyScore { get; set; }
    public int? GrammarScore { get; set; }
    public int? ReadinessScore { get; set; }
    public int? NeedsHelpScore { get; set; }

    // משוב מילולי חופשי מה-AI על ביצועי התלמיד
    public string FreeTextComment { get; set; }

    // ניתוח מעמיק של ה-AI בפורמט JSON (לשימוש עתידי)
    public string AiAnalysisJson { get; set; }

    // כל היסטוריית ההודעות (הפינג-פונג) בפורמט JSON
    public string ChatHistory { get; set; }

    // תאריך ושעת יצירת השיחה וסיום השיחה
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActiveAt { get; set; }
}