using YmKB.UI.Models;

namespace YmKB.UI.Services.Contracts;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(UserLoginDto userLoginDto);
    Task LogoutAsync();
    // Task<bool> RegisterAsync(string userName, string email, string password);
}