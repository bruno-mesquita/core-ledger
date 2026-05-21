using CoreLedger.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

namespace CoreLedger.Infrastructure.Persistence.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IEnumerable<Account>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default) =>
        await context.Accounts.Where(a => a.OwnerId == ownerId).ToListAsync(ct);

    public async Task AddAsync(Account account, CancellationToken ct = default) =>
        await context.Accounts.AddAsync(account, ct);

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        context.Accounts.Update(account);
        return Task.CompletedTask;
    }
}
