namespace BlazorGoogleLogin.Shared.DTOs;

public class StudentReportDTO
{
    public string Name { get; set; }
    public string Mail { get; set; }
    public int TotalSessions { get; set; }
    
    // ציונים ממוצעים (יכולים להיות Null אם התלמיד עוד לא התאמן)
    public double? FluencyScore { get; set; }
    public double? GrammarScore { get; set; }
    
    // השלב האחרון שהתלמיד הגיע אליו
    public int? CurrentPhase { get; set; }
    
    // תאריך הפעילות האחרונה (LastActiveAt מה-DB)
    public DateTime? LastActive { get; set; } 

    // כאן ההחלטה שלך:
    // אם ב-SQL את מחשבת ממוצע או מקסימום של ציון העזרה:
    public int? NeedsHelpScore { get; set; } 
    
    // טיפ: אם את רוצה שיהיה לך קל ב-CSS, אפשר להוסיף משתנה "מחושב" ב-DTO
    public bool IsUrgent => NeedsHelpScore > 2;
    
    //הערה חופשית של התלמיד
    public string FreeTextComment { get; set; }
    
    //נושא השיחה בו התלמיד ביקש עזרה
    public string? HelpTopic { get; set; }
}