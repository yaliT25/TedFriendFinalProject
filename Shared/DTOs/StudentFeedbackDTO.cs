namespace BlazorGoogleLogin.Shared.DTOs;

public class StudentFeedbackDTO
{
    public int Rating { get; set; } // עבור NeedsHelpScore
    public string Comment { get; set; } // עבור FreeTextComment
}