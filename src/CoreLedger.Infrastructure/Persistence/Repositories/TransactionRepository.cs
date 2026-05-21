using CoreLedger.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace CoreLedger.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public Task<Transaction?> GetByIdempotencyKeyAsync(string key, CancellationToken ct = default) =>
        context.Transactions.FirstOrDefaultAsync(t => t.IdempotencyKey == key, ct);

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(
        Guid accountId, int page, int pageSize, CancellationToken ct = default) =>
        await context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddRangeAsync(IEnumerable<Transaction> transactions, CancellationToken ct = default) =>
        await context.Transactions.AddRangeAsync(transactions, ct);

    public async Task AddLedgerEntriesAsync(IEnumerable<LedgerEntry> entries, CancellationToken ct = default) =>
        await context.LedgerEntries.AddRangeAsync(entries, ct);
}
