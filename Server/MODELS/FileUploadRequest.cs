using Microsoft.AspNetCore.Http;

namespace BlazorGoogleLogin.Server.Models // ודאי שה-Namespace מתאים לשרת שלך
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; }
    }
}