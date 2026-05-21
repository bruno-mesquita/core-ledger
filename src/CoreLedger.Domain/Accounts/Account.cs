using CoreLedger.SharedKernel;

namespace CoreLedger.Domain.Accounts;

public class Account : AggregateRoot
{
    public Guid OwnerId { get; private set; }
    public string AccountNumber { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }
    public decimal Limit { get; private set; }
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private Account() { }

    public static Account Create(Guid ownerId) =>
        new()
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            AccountNumber = GenerateAccountNumber(),
            Balance = 0m,
            Limit = 0m,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

    private static string GenerateAccountNumber() =>
        Random.Shared.Next(1_000_000_000, int.MaxValue).ToString();

    public Result Debit(decimal amount)
    {
        if (Status == AccountStatus.Closed)
            return Result.Failure("Account is closed.");
        if (amount <= 0)
            return Result.Failure("Amount must be positive.");
        if (Balance + Limit < amount)
            return Result.Failure("Insufficient funds.");

        Balance -= amount;
        return Result.Success();
    }

    public Result Credit(decimal amount)
    {
        if (Status == AccountStatus.Closed)
            return Result.Failure("Account is closed.");
        if (amount <= 0)
            return Result.Failure("Amount must be positive.");

        Balance += amount;
        return Result.Success();
    }

    public Result Close()
    {
        if (Status == AccountStatus.Closed)
            return Result.Failure("Account is already closed.");
        if (Balance != 0)
            return Result.Failure("Cannot close account with non-zero balance.");

        Status = AccountStatus.Closed;
        ClosedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public void SetLimit(decimal limit) => Limit = limit >= 0 ? limit : 0m;
}
