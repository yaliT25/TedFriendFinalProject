namespace BlazorGoogleLogin.Shared.DTOs;

public class Message
{
    public string role { get; set; }
    public string content { get; set; }
    
    public string? translatedContent { get; set; }
    public int? Phase { get; set; }
}