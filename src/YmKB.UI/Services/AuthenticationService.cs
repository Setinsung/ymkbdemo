using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using YmKB.UI.Models;
using YmKB.UI.Providers;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IStorageService _storageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public AuthenticationService(
        IStorageService storageService,
        AuthenticationStateProvider authenticationStateProvider,
        HttpClient httpClient
    )
    {
        _storageService = storageService;
        _authenticationStateProvider = authenticationStateProvider;
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<bool> LoginAsync(UserLoginDto userLoginDto)
    {
        try
        {
            var authResult = await _httpClient.PostAsJsonAsync("Auth/login", userLoginDto);
            var authContent = await authResult.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(authContent, _options);
            if (result is null || string.IsNullOrEmpty(result.Token)) return false;
            
            await _storageService.SetItemAsync("token", result.Token);
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }

    public async Task LogoutAsync()
    {
        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
    }
}
