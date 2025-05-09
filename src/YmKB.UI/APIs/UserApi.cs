using System.Net.Http.Json;
using YmKB.UI.APIs.Models;

namespace YmKB.UI.APIs;

public class UserApi
{
    private readonly HttpClient _httpClient;

    public UserApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserResponse>>("api/users");
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    public async Task<UserResponse> UpdateAsync(string id, UpdateUserRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponse>();
    }

    public async Task DeleteAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"api/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task AssignRolesAsync(string userId, List<string> roles)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/users/{userId}/roles", roles);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveRoleAsync(string userId, string role)
    {
        var response = await _httpClient.DeleteAsync($"api/users/{userId}/roles/{role}");
        response.EnsureSuccessStatusCode();
    }
}
