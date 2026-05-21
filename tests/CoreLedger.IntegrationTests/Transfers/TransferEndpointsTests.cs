using System.Net;
using System.Net.Http.Json;
using CoreLedger.Application.Accounts.DTOs;
using CoreLedger.IntegrationTests.Helpers;

namespace CoreLedger.IntegrationTests.Transfers;

public class TransferEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task Transfer_InsufficientFunds_Returns400()
    {
        var auth1 = await TestAuthHelper.RegisterAndLoginAsync(Client, $"src{Guid.NewGuid():N}@test.com", "Password1!");
        Client.SetBearerToken(auth1.AccessToken);
        var srcResp = await Client.PostAsJsonAsync("/accounts", new { });
        var src = await srcResp.Content.ReadFromJsonAsync<AccountResponse>();

        var auth2 = await TestAuthHelper.RegisterAndLoginAsync(Client, $"dst{Guid.NewGuid():N}@test.com", "Password1!");
        Client.SetBearerToken(auth2.AccessToken);
        var dstResp = await Client.PostAsJsonAsync("/accounts", new { });
        var dst = await dstResp.Content.ReadFromJsonAsync<AccountResponse>();

        Client.SetBearerToken(auth1.AccessToken);
        var response = await Client.PostAsJsonAsync("/transfers", new
        {
            SourceAccountId = src!.Id,
            DestinationAccountId = dst!.Id,
            Amount = 100m,
            Description = "test",
            IdempotencyKey = Guid.NewGuid().ToString()
        });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Transfer_InvalidAmount_Returns400()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, $"ta{Guid.NewGuid():N}@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);

        var response = await Client.PostAsJsonAsync("/transfers", new
        {
            SourceAccountId = Guid.NewGuid(),
            DestinationAccountId = Guid.NewGuid(),
            Amount = -50m,
            Description = "bad",
            IdempotencyKey = Guid.NewGuid().ToString()
        });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Transfer_Unauthenticated_Returns401()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        var response = await Client.PostAsJsonAsync("/transfers", new
        {
            SourceAccountId = Guid.NewGuid(),
            DestinationAccountId = Guid.NewGuid(),
            Amount = 100m,
            Description = "desc",
            IdempotencyKey = Guid.NewGuid().ToString()
        });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Transfer_SelfTransfer_Returns400()
    {
        var auth = await TestAuthHelper.RegisterAndLoginAsync(Client, $"self{Guid.NewGuid():N}@test.com", "Password1!");
        Client.SetBearerToken(auth.AccessToken);
        var createResp = await Client.PostAsJsonAsync("/accounts", new { });
        var account = await createResp.Content.ReadFromJsonAsync<AccountResponse>();

        var response = await Client.PostAsJsonAsync("/transfers", new
        {
            SourceAccountId = account!.Id,
            DestinationAccountId = account.Id,
            Amount = 100m,
            Description = "self",
            IdempotencyKey = Guid.NewGuid().ToString()
        });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
