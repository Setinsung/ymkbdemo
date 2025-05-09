using YmKB.Application.Models.Identity;
using YmKB.Domain.Abstractions.Identities;

namespace YmKB.Application.Contracts.Identity;

public interface IUserService
{
    Task<UserResponse> GetUserByIdAsync(string userId);
    Task<UserResponse> GetUserByEmailAsync(string email);
    Task<List<UserResponse>> GetAllUsersAsync();
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse> UpdateUserAsync(string userId, UpdateUserRequest request);
    Task DeleteUserAsync(string userId);
    Task<bool> AssignRoleAsync(string userId, string roleName);
    Task<bool> RemoveRoleAsync(string userId, string roleName);
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<bool> ResetPasswordAsync(string userId, string newPassword);
}
