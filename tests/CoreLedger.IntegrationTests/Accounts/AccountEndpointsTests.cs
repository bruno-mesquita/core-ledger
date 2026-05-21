using System.Net;
using System.Net.Http.Json;
using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.IntegrationTests.Helpers;

namespace CoreLedger.IntegrationTests.Accounts;

public class AccountEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateAccount_Authenticated_Returns201()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, "acc1@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);

        var response = await Client.PostAsJsonAsync("/accounts", new { });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var account = await response.Content.ReadFromJsonAsync<AccountResponse>();
        account!.Balance.Should().Be(0);
        account.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateAccount_Unauthenticated_Returns401()
    {
        var response = await Client.PostAsJsonAsync("/accounts", new { });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAccount_Owner_Returns200()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, "get@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);
        var createResp = await Client.PostAsJsonAsync("/accounts", new { });
        var account = await createResp.Content.ReadFromJsonAsync<AccountResponse>();

        var response = await Client.GetAsync($"/accounts/{account!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CloseAccount_ZeroBalance_Returns204()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, "close@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);
        var createResp = await Client.PostAsJsonAsync("/accounts", new { });
        var account = await createResp.Content.ReadFromJsonAsync<AccountResponse>();

        var response = await Client.DeleteAsync($"/accounts/{account!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetBalance_Owner_Returns200()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, "bal@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);
        var createResp = await Client.PostAsJsonAsync("/accounts", new { });
        var account = await createResp.Content.ReadFromJsonAsync<AccountResponse>();

        var response = await Client.GetAsync($"/accounts/{account!.Id}/balance");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
