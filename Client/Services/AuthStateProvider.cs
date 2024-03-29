﻿using ApeGama.Shared;
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
                        new Claim(ClaimTypes.Role, "Customer")
                    }, "Customer");
                }
                else if (model.userType == 3)
                {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, model.userName),
                        new Claim(ClaimTypes.Email, model.userEmail),
                        new Claim(ClaimTypes.Role, "Admin")
                    }, "Admin");
                }
                else
                {
                    identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, model.userName),
                        new Claim(ClaimTypes.Email, model.userEmail),
                        new Claim(ClaimTypes.Role, "Supplier")
                    }, "Supplier");
                }
                state = new AuthenticationState(new ClaimsPrincipal(identity));
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
    }
}
