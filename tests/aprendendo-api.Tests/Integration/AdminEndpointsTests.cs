using System.Net;
using aprendendo_api.Tests.Helpers;

namespace aprendendo_api.Tests.Integration;

public class AdminEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AdminEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AdminTest_AsAdmin_Returns200()
    {
        var token = await TestAuthHelper.GetAdminTokenAsync(_client);
        _client.SetBearerToken(token);

        var response = await _client.GetAsync("/admin/test");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminTest_AsUser_Returns403()
    {
        var token = await TestAuthHelper.RegisterAndLoginAsync(_client, "plain@example.com", "Password123");
        _client.SetBearerToken(token);

        var response = await _client.GetAsync("/admin/test");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AdminTest_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/admin/test");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
