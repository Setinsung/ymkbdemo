using Microsoft.AspNetCore.Components.Authorization;
using YMKB.UI.APIs;
using YMKB.UI.APIs.Models;
using YmKB.UI.Providers;
using YmKB.UI.Services.Contracts;

namespace YmKB.UI.Services;

public class AuthenticationService
{
    private readonly IStorageService _storageService;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ApiClient _apiClient;

    public AuthenticationService(
        IStorageService storageService,
        AuthenticationStateProvider authenticationStateProvider,
        ApiClient apiClient
    )
    {
        _storageService = storageService;
        _authenticationStateProvider = authenticationStateProvider;
        _apiClient = apiClient;
    }

    public async Task<bool> LoginAsync(AuthRequest authRequest, bool remember = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _apiClient.Auth.Login.PostAsync(authRequest, cancellationToken: cancellationToken);
            if (result is null || string.IsNullOrEmpty(result.Token)) return false;
            await _storageService.SetItemAsync("token", result.Token);
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
            return true;
        }
        // catch (ProblemDetails)
        // {
        //     throw;
        // }
        // catch (ApiException)
        // {
        //     // Log and re-throw API exception
        //     throw;
        // }
        catch (Exception)
        {
            // Log and re-throw general exception
            // throw;
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
    }
}
