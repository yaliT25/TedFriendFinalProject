namespace BlazorGoogleLogin.Shared.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string? GoogleID { get; set; }
        public int? Role { get; set; }          // 1 למורה, 2 לתלמיד
        public string? ClassCode { get; set; }  // קוד הכיתה של המורה
        public int? TeacherId { get; set; }     // המזהה של המורה (עבור תלמידים)
    }
}
