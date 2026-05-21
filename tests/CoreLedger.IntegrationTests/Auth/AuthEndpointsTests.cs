using System.Net;
using System.Net.Http.Json;
using CoreLedger.Application.Auth.DTOs;
using CoreLedger.IntegrationTests.Helpers;

namespace CoreLedger.IntegrationTests.Auth;

public class AuthEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task Register_ValidData_Returns201WithTokens()
    {
        var response = await Client.PostAsJsonAsync("/auth/register",
            new { Email = "reg@test.com", Password = "Password1!" });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrEmpty();
        auth.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var payload = new { Email = "dup@test.com", Password = "Password1!" };
        await Client.PostAsJsonAsync("/auth/register", payload);
        var response = await Client.PostAsJsonAsync("/auth/register", payload);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200()
    {
        await Client.PostAsJsonAsync("/auth/register",
            new { Email = "login@test.com", Password = "Password1!" });
        var response = await Client.PostAsJsonAsync("/auth/login",
            new { Email = "login@test.com", Password = "Password1!" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        await Client.PostAsJsonAsync("/auth/register",
            new { Email = "wrong@test.com", Password = "Password1!" });
        var response = await Client.PostAsJsonAsync("/auth/login",
            new { Email = "wrong@test.com", Password = "WrongPass!" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Refresh_ValidToken_Returns200WithNewTokens()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, "refresh@test.com", "Password1!");
        var response = await Client.PostAsJsonAsync("/auth/refresh", new { Token = auth.RefreshToken });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var newAuth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        newAuth!.AccessToken.Should().NotBe(auth.AccessToken);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/auth/register",
            new { Email = "not-an-email", Password = "Password1!" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
