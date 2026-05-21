using CoreLedger.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreLedger.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired();

        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed admin user — BCrypt hash for "Admin@123" with workFactor 11
        builder.HasData(new
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Email = "admin@coreledger.com",
            PasswordHash = "$2a$11$UtSPv8lsxLhuuaxY3Ly3puAAwLkEFVTARmWyDDGzSl/nqa5SSDyOy",
            Role = Role.Admin
        });
    }
}
