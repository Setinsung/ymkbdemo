namespace YmKB.UI.APIs.Models;

public class UserResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class CreateUserRequest
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UpdateUserRequest
{
    public string Email { get; set; }
}

public class RoleResponse
{
    public string Name { get; set; }
    public bool IsSelected { get; set; }
}
