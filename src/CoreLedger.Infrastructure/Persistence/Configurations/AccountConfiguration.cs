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
        builder.Property(a => a.RowVersion).IsRowVersion();
    }
}
