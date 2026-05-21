using CoreLedger.SharedKernel;

namespace CoreLedger.Domain.Transactions;

public class LedgerEntry : Entity
{
    public Guid TransactionId { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }
    public decimal RunningBalance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private LedgerEntry() { }

    public static LedgerEntry ForDebit(Guid transactionId, Guid accountId, decimal amount, decimal runningBalance) =>
        new()
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            AccountId = accountId,
            Debit = amount,
            Credit = 0m,
            RunningBalance = runningBalance,
            CreatedAt = DateTime.UtcNow
        };

    public static LedgerEntry ForCredit(Guid transactionId, Guid accountId, decimal amount, decimal runningBalance) =>
        new()
        {
            Id = Guid.NewGuid(),
            TransactionId = transactionId,
            AccountId = accountId,
            Debit = 0m,
            Credit = amount,
            RunningBalance = runningBalance,
            CreatedAt = DateTime.UtcNow
        };
}
