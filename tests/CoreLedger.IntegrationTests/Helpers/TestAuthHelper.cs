using System.Net.Http.Json;
using CoreLedger.Application.Auth.DTOs;

namespace CoreLedger.IntegrationTests.Helpers;

public static class TestAuthHelper
{
    public static async Task<AuthResponse> RegisterAndLoginAsync(
        HttpClient client, string email, string password)
    {
        await client.PostAsJsonAsync("/auth/register",
            new { Email = email, Password = password });
        var response = await client.PostAsJsonAsync("/auth/login",
            new { Email = email, Password = password });
        return (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
    }

    public static void SetBearerToken(this HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
}
