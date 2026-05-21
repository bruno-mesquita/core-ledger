using CoreLedger.Domain.Accounts;

namespace CoreLedger.Application.Accounts.DTOs;

public record AccountResponse(
    Guid Id,
    string AccountNumber,
    decimal Balance,
    decimal Limit,
    string Status,
    DateTime CreatedAt)
{
    public static AccountResponse FromEntity(Account account) =>
        new(account.Id,
            account.AccountNumber,
            account.Balance,
            account.Limit,
            account.Status.ToString(),
            account.CreatedAt);
}
