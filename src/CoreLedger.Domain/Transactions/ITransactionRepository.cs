namespace CoreLedger.Domain.Transactions;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId, int page, int pageSize, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default);
    Task AddLedgerEntriesAsync(IEnumerable<LedgerEntry> entries, CancellationToken ct = default);
}
