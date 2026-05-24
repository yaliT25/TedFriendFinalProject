using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite; // חבילה ל-SQLite
using Dapper;               // חבילה ל-Dapper
using System.Security.Claims;
using BlazorGoogleLogin.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BlazorGoogleLogin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _config;

        // הזרקת ה-Configuration כדי שנוכל למשוך את ה-Connection String
        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult GetLoginUser()
        {
            var token = Request.Cookies["jwt_token"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            return Ok(token);
        }

        [HttpPost("update-role")]
        [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] string roleName)
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            try 
            {
                using (var connection = new SqliteConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    // שולפים גם את ה-TeacherId כדי לדעת אם התלמידה כבר הצטרפה למורה
                    var currentUser = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT Role, ClassCode, TeacherId FROM Users WHERE Id = @userId", new { userId = userIdClaim });

                    int roleId = (roleName == "Teacher") ? 1 : 2;
                    string? classCode = currentUser?.ClassCode; 

                    // לוגיקה למורה: מייצרים קוד אם חסר
                    if (roleId == 1 && string.IsNullOrEmpty(classCode))
                    {
                        classCode = GenerateClassCode();
                    }
            
                    // לוגיקה לתלמידה: אם יש לה TeacherId, אנחנו נשלח לממשק "סימן" שהיא משויכת
                    if (roleId == 2 && currentUser?.TeacherId != null)
                    {
                        // אנחנו שמים ערך כלשהו ב-classCode כדי שהממשק יבין שהיא לא צריכה StudentEntry
                        classCode = "JOINED"; 
                    }

                    string sql = "UPDATE Users SET Role = @roleId, ClassCode = @classCode WHERE Id = @userId";
                    await connection.ExecuteAsync(sql, new { roleId, classCode, userId = userIdClaim });
            
                    return Ok(new ClassCodeResponseDTO 
                    { 
                        Code = classCode,
                        ClassCode = classCode 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        
        [HttpGet("my-info")]
        [Authorize]
        public async Task<IActionResult> GetMyInfo()
        {
            // 1. שליפת ה-ID מהטוקן (מחפשים sub כי זה מה ששמנו ב-TokenService)
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            try 
            {
                // 2. תיקון שם ה-Connection String ל-DefaultConnection
                using (var connection = new SqliteConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    // 3. שליפת הנתונים - ודאי ששמות העמודות (Role, ClassCode) תואמים ל-DB שלך
                    string sql = "SELECT Role, ClassCode AS Code FROM Users WHERE Id = @userId";

                    var user = await connection.QueryFirstOrDefaultAsync<ClassCodeResponseDTO>(sql, new { userId = userIdClaim });

                    if (user == null) return NotFound("User not found in database.");

                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                // זה יעזור לך לראות ב-Console של השרת מה הבעיה האמיתית אם זה עדיין קורס
                return StatusCode(500, $"Database Error: {ex.Message}");
            }
        }

        [HttpPost("join-class")]
        [Authorize]
        public async Task<IActionResult> JoinClass([FromBody] object rawCode)
        {
            // 1. זיהוי משתמשת - משתמשים בדיוק באותה שורה שעבדה ב-UpdateRole
            var studentId = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return Unauthorized("השרת לא מזהה את המשתמשת בטוקן.");
            }

            // 2. חילוץ הקוד - וידוא שהטקסט מגיע נקי
            string code = rawCode.ToString();

            try
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    // 3. חיפוש המורה לפי הקוד
                    string findTeacherSql = "SELECT Id FROM Users WHERE ClassCode = @code AND Role = 1";
                    var teacherId = await connection.QueryFirstOrDefaultAsync<int?>(findTeacherSql, new { code });

                    if (teacherId == null)
                    {
                        return BadRequest("קוד כיתה לא נמצא.");
                    }

                    // 4. עדכון התלמידה עם ה-TeacherID
                    string updateStudentSql = "UPDATE Users SET TeacherId = @teacherId WHERE Id = @studentId";
                    await connection.ExecuteAsync(updateStudentSql, new { teacherId, studentId });

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"שגיאת שרת: {ex.Message}");
            }
        }
        
        [HttpGet("my-students-report")]
        [Authorize]
        public async Task<IActionResult> GetMyStudentsReport()
        {
            // שליפת ה-ID של המורה מהטוקן
            var teacherId = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            try
            {
                using (var connection = new Microsoft.Data.Sqlite.SqliteConnection(_config.GetConnectionString("DefaultConnection")))
                {
                    // שאילתה לשליפת נתונים וחישובים משתי טבלאות
                    string sql = @"
    SELECT 
    u.Name, 
    u.Mail, 
    COUNT(cs.Id) AS TotalSessions,
    AVG(cs.GrammarScore) AS GrammarScore,
    AVG(cs.FluencyScore) AS FluencyScore,
    
    
    (SELECT CurrentPhase FROM ChatSessions WHERE UserId = u.Id ORDER BY CreatedAt DESC LIMIT 1) AS CurrentPhase,
    
    
    lh.FreeTextComment,
    lh.Topic AS HelpTopic,

    MAX(cs.LastActiveAt) AS LastActive,
    MAX(cs.NeedsHelpScore) AS NeedsHelpScore 
FROM Users u
LEFT JOIN ChatSessions cs ON u.Id = cs.UserId


LEFT JOIN (
    SELECT UserId, FreeTextComment, Topic
    FROM ChatSessions 
    WHERE NeedsHelpScore = 3
    GROUP BY UserId
    HAVING MAX(CreatedAt) /* טריק של SQLite לשליפת השורה האחרונה בקבוצה */
) lh ON u.Id = lh.UserId

WHERE u.TeacherId = @teacherId
GROUP BY u.Id, u.Name, u.Mail";

                    var report = await connection.QueryAsync<StudentReportDTO>(sql, new { teacherId });
                    return Ok(report);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database Error: {ex.Message}");
            }
        }

        // --- פונקציית העזר ליצירת קוד רנדומלי ---
        private string GenerateClassCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}