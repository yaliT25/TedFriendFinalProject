using System.Collections.Generic;
namespace BlazorGoogleLogin.Shared.DTOs;

public class ChatRequest
{
    public string DocumentContent { get; set; } // הטקסט מה-PDF
    public List<Message> History { get; set; } = new List<Message>();
}