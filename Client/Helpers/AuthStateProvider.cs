using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using System.Net;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace BlazorGoogleLogin.Client.Helpers
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "api/users");
            requestMessage.SetBrowserRequestCredentials(BrowserRequestCredentials.Include); // ← זה החלק החדש

            var get = await _httpClient.SendAsync(requestMessage);

            if (get.IsSuccessStatusCode)
            {
                string token = await get.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(token))
                    return _anonymous;

                token = token.Trim().Trim('"');

                if (string.IsNullOrWhiteSpace(token))
                    return _anonymous;

                return new AuthenticationState(
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            JwtParser.ParseClaimsFromJwt(token),
                            "jwtAuthType"
                        )
                    )
                );
            }
            return _anonymous;
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}