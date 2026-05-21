using System.Net.Http.Json;
using aprendendo_api.Application.DTOs;

namespace aprendendo_api.Tests.Helpers;

public static class TestAuthHelper
{
    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string email, string password, string role = "User")
    {
        await client.PostAsJsonAsync("/auth/register", new RegisterRequest(email, password));
        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest(email, password));
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }

    public static async Task<string> GetAdminTokenAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/auth/login",
            new LoginRequest("admin@example.com", "Admin@123"));
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }

    public static void SetBearerToken(this HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
}
