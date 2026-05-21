using CoreLedger.SharedKernel;

namespace CoreLedger.Domain.Transactions;

public class Transaction : Entity
{
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionKind Kind { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string IdempotencyKey { get; private set; } = string.Empty;
    public Guid? RelatedAccountId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction CreateDebit(
        Guid accountId,
        decimal amount,
        string description,
        string idempotencyKey,
        TransactionKind kind,
        Guid? relatedAccountId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Amount = amount,
            Type = TransactionType.Debit,
            Kind = kind,
            Description = description,
            IdempotencyKey = idempotencyKey,
            RelatedAccountId = relatedAccountId,
            CreatedAt = DateTime.UtcNow
        };

    public static Transaction CreateCredit(
        Guid accountId,
        decimal amount,
        string description,
        string idempotencyKey,
        TransactionKind kind,
        Guid? relatedAccountId = null) =>
        new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Amount = amount,
            Type = TransactionType.Credit,
            Kind = kind,
            Description = description,
            IdempotencyKey = idempotencyKey,
            RelatedAccountId = relatedAccountId,
            CreatedAt = DateTime.UtcNow
        };
}
