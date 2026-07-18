namespace BlazorGoogleLogin.Shared.DTOs;

public class ClassCodeResponseDTO
{
    public string? Code { get; set; }
    public string ClassCode { get; set; }
    public int? Role { get; set; }
    public int? TeacherId { get; set; }
}