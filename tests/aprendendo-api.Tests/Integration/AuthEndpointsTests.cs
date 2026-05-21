using System.Net;
using System.Net.Http.Json;
using aprendendo_api.Application.DTOs;
using aprendendo_api.Tests.Helpers;

namespace aprendendo_api.Tests.Integration;

public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("new@example.com", "Password123"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth?.Token);
        Assert.Equal("new@example.com", auth.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("dup@example.com", "Password123"));
        var response = await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("dup@example.com", "Password123"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("login@example.com", "Password123"));

        var response = await _client.PostAsJsonAsync("/auth/login",
            new LoginRequest("login@example.com", "Password123"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth?.Token);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        await _client.PostAsJsonAsync("/auth/register",
            new RegisterRequest("wrong@example.com", "Password123"));

        var response = await _client.PostAsJsonAsync("/auth/login",
            new LoginRequest("wrong@example.com", "WrongPass"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/auth/login",
            new LoginRequest("ghost@example.com", "Password123"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
