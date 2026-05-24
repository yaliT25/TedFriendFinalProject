namespace BlazorGoogleLogin.Shared.DTOs;

public class AiAnalysisResult
{
    public int FluencyScore { get; set; }
    public int GrammarScore { get; set; }
    public string Keep { get; set; }
    public string Improve { get; set; }
}