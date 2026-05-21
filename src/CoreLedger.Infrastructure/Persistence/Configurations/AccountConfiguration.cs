using CoreLedger.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreLedger.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AccountNumber).IsRequired().HasMaxLength(20);
        builder.HasIndex(a => a.AccountNumber).IsUnique();
        builder.Property(a => a.Balance).HasPrecision(18, 4);
        builder.Property(a => a.Limit).HasPrecision(18, 4);
        builder.Property(a => a.Status).IsRequired();

        // PostgreSQL/Npgsql requires uint (mapped to xmin system column) for IsRowVersion().
        // The domain model uses byte[] which is not auto-generated in PostgreSQL.
        // We ignore RowVersion here and rely on application-level concurrency handling.
        builder.Ignore(a => a.RowVersion);
    }
}
