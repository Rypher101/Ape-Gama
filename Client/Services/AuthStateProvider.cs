using ApeGama.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApeGama.Client.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;

        public AuthStateProvider(HttpClient http)
        {
            _http = http;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = new AuthenticationState(new ClaimsPrincipal());
            LoginModel model = await _http.GetFromJsonAsync<LoginModel>("api/Login");

            if (!string.IsNullOrWhiteSpace(model.userName))
            {
                ClaimsIdentity identity;
                if (model.userType == 1)
                {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, model.userName),
                        new Claim(ClaimTypes.Email, model.userEmail),
                        new Claim(ClaimTypes.Role, "C")
                    },"Customer");
                }
                else
                {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, model.userName),
                        new Claim(ClaimTypes.Email, model.userEmail),
                        new Claim(ClaimTypes.Role, "S")
                    }, "Supplier");
                }
                state = new AuthenticationState(new ClaimsPrincipal(identity));
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
    }
}
