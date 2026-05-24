using System;

namespace BlazorGoogleLogin.Client.Services
{
    public class SessionState
    {
        public int SelectedSessionId { get; set; }
        public int CurrentPhase { get; set; } = 1;
        public string DocumentContext { get; set; }
        public string Topic { get; set; } 

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void InitializedSession(int id, string context, string topic)
        {
            SelectedSessionId = id;
            DocumentContext = context;
            Topic = topic; // שמירת הנושא שה-AI יצר או שהמשתמש הזין
            CurrentPhase = 2; 
            
            NotifyStateChanged(); 
        }

        // פונקציה חשובה ליציאה מסודרת שתנקה את הזיכרון
        public void Clear()
        {
            SelectedSessionId = 0;
            CurrentPhase = 1;
            DocumentContext = null;
            Topic = null;
            NotifyStateChanged();
        }
    }
}