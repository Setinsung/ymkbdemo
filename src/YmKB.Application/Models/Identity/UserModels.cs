namespace YmKB.Application.Models.Identity;

public class UserResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class CreateUserRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}

public class UpdateUserRequest
{
    public string? Email { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public class RegisterRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}
