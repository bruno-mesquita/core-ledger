using CoreLedger.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreLedger.Infrastructure.Persistence.Configurations;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.HasKey(le => le.Id);
        builder.Property(le => le.Debit).HasPrecision(18, 4);
        builder.Property(le => le.Credit).HasPrecision(18, 4);
        builder.Property(le => le.RunningBalance).HasPrecision(18, 4);
        builder.HasIndex(le => le.AccountId);
        builder.HasIndex(le => le.TransactionId);
    }
}
