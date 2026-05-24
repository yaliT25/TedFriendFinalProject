using System.Collections.Generic;
namespace BlazorGoogleLogin.Shared.DTOs;

public class GPTRequest
{
    public string model { get; set; }
    public int max_output_tokens { get; set; }
    public double temperature { get; set; }
    public object text { get; set; } // כאן נגדיר את ה-JSON Schema אם נרצה תשובה מובנית
    public List<Message> input { get; set; } = new List<Message>();
}