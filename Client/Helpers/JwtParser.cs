using System.Security.Claims;
using System.Text.Json;
using System.Linq;

namespace BlazorGoogleLogin.Client.Helpers
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            if (string.IsNullOrWhiteSpace(jwt))
                return claims;

            // ניקוי יסודי של הטוקן - הסרת גרשיים ורווחים שעלולים להגיע מה-LocalStorage
            jwt = jwt.Trim().Replace("\"", "");

            if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                jwt = jwt.Substring(7).Trim();
            }

            var segments = jwt.Split('.');
            if (segments.Length < 2)
                return claims;

            try 
            {
                var payload = segments[1];
                var jsonBytes = ParseBase64WithoutPadding(payload);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

                if (keyValuePairs != null)
                {
                    claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? "")));
                }
            }
            catch (Exception ex)
            {
                // אם עדיין יש שגיאה, נדפיס אותה לקונסול כדי שנראה מה הבעיה
                Console.WriteLine("JWT Parsing Error: " + ex.Message);
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            // 1. ניקוי תווים לא חוקיים ורווחים מהחלק הספציפי
            base64 = base64.Trim();

            // 2. תיקון פורמט Base64Url ל-Base64 סטנדרטי
            base64 = base64.Replace('-', '+').Replace('_', '/');

            // 3. הוספת ריפוד (Padding) רק אם חסר
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}