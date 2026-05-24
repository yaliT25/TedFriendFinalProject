using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorGoogleLogin.Server.Helpers
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(ClaimsPrincipal principal, int userId)
        {
            // 1. שליפת נתונים בסיסיים מה-Principal של גוגל
            var name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue("name");
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue("email");

            // 2. בניית ה-Claims - כאן אנחנו מסדרים את ה"זהות" של המשתמשת בטוקן
            var claims = new List<Claim>
            {
                // ה-Sub הוא המזהה הייחודי. אנחנו שמים בו את ה-ID המספרי מה-DB (כמחרוזת)
                // זה מה שימנע את השגיאה בשורה 44 ב-AuthenticationService
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email ?? ""),
                new Claim("name", name ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // מזהה ייחודי לטוקן עצמו למניעת Replay attacks
            };

            // 3. שליפת הגדרות האבטחה מה-Configuration
            var securityKey = _configuration["JWTSettings:securityKey"];
            var issuer = _configuration["JWTSettings:validIssuer"];
            var audience = _configuration["JWTSettings:validAudience"];

            // בדיקת תקינות למפתח האבטחה
            if (string.IsNullOrEmpty(securityKey) || securityKey.Length < 32)
            {
                throw new Exception("JWT Security Key is missing or too short in appsettings.json");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 4. יצירת האובייקט של הטוקן עם תוקף ל-7 ימים
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            // 5. חתימה דיגיטלית והפיכה למחרוזת
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshToken(string token)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}