using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Providers;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IStorageService _storageService;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    public ApiAuthenticationStateProvider(IStorageService storageService)
    {
        _storageService = storageService;
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    }
    
    public async Task LoggedIn()
    {
        var savedToken = await _storageService.GetItemAsync<string>("token");
        var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
        var user = GetClaimsPrincipal(tokenContent);
        var authState = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authState);
    }

    public async Task LoggedOut()
    {
        await _storageService.RemoveItemAsync("token");
        var nobody = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(nobody));
        NotifyAuthenticationStateChanged(authState);
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var isTokenPresent = await _storageService.ContainKeyAsync("token");
        if (isTokenPresent == false)
        {
            return new AuthenticationState(user);
        }

        var savedToken = await _storageService.GetItemAsync<string>("token");
        var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

        if (tokenContent.ValidTo < DateTime.UtcNow) // token解析出来的到期时间是utc时间
        {
            await _storageService.RemoveItemAsync("token");
            return new AuthenticationState(user);
        }
        user = GetClaimsPrincipal(tokenContent);
        return new AuthenticationState(user);
    }

    private ClaimsPrincipal GetClaimsPrincipal(JwtSecurityToken tokenContent)
    {
        var claims = tokenContent.Claims.ToList();
        claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }
}