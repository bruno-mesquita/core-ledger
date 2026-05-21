using CoreLedger.Domain.Transactions;

namespace CoreLedger.Application.Accounts.DTOs;

public record TransactionResponse(
    Guid Id,
    decimal Amount,
    string Type,
    string Kind,
    string Description,
    DateTime CreatedAt)
{
    public static TransactionResponse FromEntity(Transaction t) =>
        new(t.Id, t.Amount, t.Type.ToString(), t.Kind.ToString(), t.Description, t.CreatedAt);
}
