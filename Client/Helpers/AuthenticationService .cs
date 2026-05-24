using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorGoogleLogin.Shared.DTOs;

namespace BlazorGoogleLogin.Client.Helpers
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient client, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _httpClient = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
        }

        public async Task Logout()
        {
            var get = await _httpClient.GetAsync("api/users/logout");
            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<UserDTO> GetUserFromClaimAsync()
        {
            var authenticationState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authenticationState.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                // שליפת ה-Claim של ה-sub (שבתוכו אמור להיות ה-ID)
                var subClaim = user.FindFirst(c => c.Type == "sub")?.Value;

                // ניסיון המרה בטוח ל-short (Int16)
                // אם subClaim הוא לא מספר (למשל אם הוא מכיל מייל מהעוגייה הישנה), 
                // הפונקציה תחזיר false וה-userId יישאר 0 במקום לשבור את האפליקציה.
                short.TryParse(subClaim, out short userId);

                var userDto = new UserDTO
                {
                    Name = user.FindFirst(c => c.Type == "name")?.Value,
                    Mail = user.FindFirst(c => c.Type == "email")?.Value,
                    Id = userId // מקבל את הערך שהצלחנו להמיר או 0
                };

                return userDto;
            }

            return null;
        }
    }
}