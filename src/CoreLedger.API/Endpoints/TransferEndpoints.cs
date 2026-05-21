using System.Security.Claims;
using CoreLedger.Application.Transfers.Commands.CreateTransfer;
using MediatR;

namespace CoreLedger.API.Endpoints;

public static class TransferEndpoints
{
    public static void MapTransferEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/transfers").RequireAuthorization();

        group.MapPost("/", async (CreateTransferRequest request, HttpContext ctx, ISender sender) =>
        {
            var userId = GetUserId(ctx);
            var command = new CreateTransferCommand(
                request.SourceAccountId,
                request.DestinationAccountId,
                request.Amount,
                request.Description,
                request.IdempotencyKey,
                userId);

            var result = await sender.Send(command);
            return result.IsSuccess
                ? Results.Created($"/transfers/{result.Value!.TransactionId}", result.Value)
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

public record CreateTransferRequest(
    Guid SourceAccountId,
    Guid DestinationAccountId,
    decimal Amount,
    string Description,
    string IdempotencyKey);
