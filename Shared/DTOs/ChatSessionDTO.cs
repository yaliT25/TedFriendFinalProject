namespace BlazorGoogleLogin.Shared.DTOs;

public class ChatSessionDTO
{
    public int Id { get; set; }
    public string Topic { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public int? CurrentPhase { get; set; }
    public int? FluencyScore { get; set; }
    public int? GrammarScore { get; set; }
    public string AiAnalysisJson { get; set; }
}