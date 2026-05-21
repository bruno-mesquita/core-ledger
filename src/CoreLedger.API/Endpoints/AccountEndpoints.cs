using System.Security.Claims;
using CoreLedger.Application.Accounts.Commands.CloseAccount;
using CoreLedger.Application.Accounts.Commands.CreateAccount;
using CoreLedger.Application.Accounts.Queries.GetAccount;
using CoreLedger.Application.Accounts.Queries.GetBalance;
using CoreLedger.Application.Accounts.Queries.GetTransactions;
using MediatR;

namespace CoreLedger.API.Endpoints;

public static class AccountEndpoints
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/accounts").RequireAuthorization();

        group.MapPost("/", async (HttpContext ctx, ISender sender) =>
        {
            var userId = GetUserId(ctx);
            var result = await sender.Send(new CreateAccountCommand(userId));
            return result.IsSuccess
                ? Results.Created($"/accounts/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        });

        group.MapGet("/{id:guid}", async (Guid id, HttpContext ctx, ISender sender) =>
        {
            var userId = GetUserId(ctx);
            var result = await sender.Send(new GetAccountQuery(id, userId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        });

        group.MapGet("/{id:guid}/balance", async (Guid id, HttpContext ctx, ISender sender) =>
        {
            var userId = GetUserId(ctx);
            var result = await sender.Send(new GetBalanceQuery(id, userId));
            return result.IsSuccess
                ? Results.Ok(new { balance = result.Value })
                : Results.NotFound(new { error = result.Error });
        });

        group.MapGet("/{id:guid}/transactions", async (
            Guid id, HttpContext ctx, ISender sender,
            int page = 1, int pageSize = 20) =>
        {
            var userId = GetUserId(ctx);
            var result = await sender.Send(new GetTransactionsQuery(id, userId, page, pageSize));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        });

        group.MapDelete("/{id:guid}", async (Guid id, HttpContext ctx, ISender sender) =>
        {
            var userId = GetUserId(ctx);
            var result = await sender.Send(new CloseAccountCommand(id, userId));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        });
    }

    private static Guid GetUserId(HttpContext ctx)
    {
        var sub = ctx.User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
               ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? string.Empty;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}
