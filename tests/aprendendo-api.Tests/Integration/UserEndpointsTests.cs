using System.Net;
using System.Net.Http.Json;
using aprendendo_api.Application.DTOs;
using aprendendo_api.Tests.Helpers;

namespace aprendendo_api.Tests.Integration;

public class UserEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthAsAdminAsync()
    {
        var token = await TestAuthHelper.GetAdminTokenAsync(_client);
        _client.SetBearerToken(token);
    }

    private async Task AuthAsUserAsync(string email = "user@example.com")
    {
        var token = await TestAuthHelper.RegisterAndLoginAsync(_client, email, "Password123");
        _client.SetBearerToken(token);
    }

    [Fact]
    public async Task GetUsers_WithValidToken_Returns200()
    {
        await AuthAsAdminAsync();
        var response = await _client.GetAsync("/users");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetUsers_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/users");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserById_ExistingUser_Returns200()
    {
        await AuthAsAdminAsync();
        var response = await _client.GetAsync("/users/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();
        Assert.Equal("admin@example.com", user?.Email);
    }

    [Fact]
    public async Task GetUserById_NonExistentUser_Returns404()
    {
        await AuthAsAdminAsync();
        var response = await _client.GetAsync("/users/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_AsAdmin_Returns204()
    {
        await AuthAsAdminAsync();
        var registerResponse = await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("todelete@example.com", "Password123"));
        var created = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var usersResponse = await _client.GetAsync("/users");
        var users = await usersResponse.Content.ReadFromJsonAsync<List<UserResponse>>();
        var targetId = users!.First(u => u.Email == "todelete@example.com").Id;

        var deleteResponse = await _client.DeleteAsync($"/users/{targetId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_AsNonAdmin_Returns403()
    {
        await AuthAsUserAsync("regularuser@example.com");
        var response = await _client.DeleteAsync("/users/1");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
