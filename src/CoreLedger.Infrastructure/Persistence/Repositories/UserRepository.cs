using CoreLedger.Domain.Users;
using CoreLedger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoreLedger.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByRefreshTokenAsync(string token, CancellationToken ct = default) =>
        context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token), ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await context.Users.AddAsync(user, ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        // Temporarily disable auto-detect changes so we can inspect state without triggering it
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        try
        {
            // If entity is detached, attach it as Modified
            var userEntry = context.Entry(user);
            if (userEntry.State == EntityState.Detached)
                userEntry.State = EntityState.Modified;

            // Find any new RefreshTokens (those not yet tracked by EF).
            // We must do this before SaveChangesAsync calls DetectChanges, which would
            // mark new tokens as Modified (non-zero Guid key) causing concurrency exception.
            foreach (var token in user.RefreshTokens)
            {
                var entry = context.Entry(token);
                if (entry.State == EntityState.Detached)
                    entry.State = EntityState.Added;
            }
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        context.Users.AnyAsync(u => u.Email == email, ct);
}
