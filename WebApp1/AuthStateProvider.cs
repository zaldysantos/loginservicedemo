using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp1
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly LocalStorage _localStorage; 

        public AuthStateProvider(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsIdentity();
            var userInfo = await _localStorage.GetItem<Dictionary<string, string>>("userInfo");
            if (userInfo != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userInfo["username"]),
                    new Claim(ClaimTypes.Role, "user")
                };
                user = new ClaimsIdentity(claims, userInfo["authenticationCode"]); // user
            }
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user))); // guest
        }
    }
}
