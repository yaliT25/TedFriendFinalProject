using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorGoogleLogin.Server.Helpers;
using BlazorGoogleLogin.Data;
using BlazorGoogleLogin.Shared.DTOs;

namespace BlazorGoogleLogin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly DbRepository _db;

        public GoogleController(TokenService tokenService, DbRepository db)
        {
            _tokenService = tokenService;
            _db = db;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var props = new AuthenticationProperties 
            { 
                RedirectUri = "/api/google/signin-google",
                Parameters = { { "prompt", "select_account" } } 
            };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleSignIn()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return BadRequest("Google authentication failed");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 1. חיפוש המשתמש בדאטה-בייס
            string checkQuery = "SELECT * FROM Users WHERE Mail = @Email";
            var users = await _db.GetRecordsAsync<UserDTO>(checkQuery, new { Email = email });
            var user = users?.FirstOrDefault();

            // 2. אם המשתמש לא קיים - ניצור אותו
            if (user == null)
            {
                string insertQuery = "INSERT INTO Users (Name, Mail, GoogleID) VALUES (@Name, @Mail, @GoogleID)";
                await _db.SaveDataAsync(insertQuery, new { Name = name, Mail = email, GoogleID = googleId });
                
                // שליפה מחדש כדי לקבל את ה-ID האוטומטי שנוצר ב-DB
                users = await _db.GetRecordsAsync<UserDTO>(checkQuery, new { Email = email });
                user = users?.FirstOrDefault();
            }

            // הגנה למקרה חריג שהשליפה נכשלה
            if (user == null) return StatusCode(500, "Failed to process user in database");

            // 3. יצירת הטוקן - עכשיו אנחנו שולחים את המשתמש וגם את ה-ID שלו (user.Id)
            // זה מה שמתקן את השגיאה Method 'CreateToken' has 2 parameter(s)
            var token = _tokenService.CreateToken(result.Principal, user.Id);

            // 4. שמירת הטוקן בעוגייה
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Redirect("/Users");
        }
        
    }
}