using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace YmKB.UI.Providers;

public class TestAuthStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(1500);
        var anonymous = new ClaimsIdentity();
        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
    }
}