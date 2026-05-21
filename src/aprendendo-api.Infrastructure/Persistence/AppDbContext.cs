using aprendendo_api.Domain.Entities;
using aprendendo_api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace aprendendo_api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasData(new
            {
                Id = 1,
                Email = "admin@example.com",
                PasswordHash = "$2a$12$EBaIFDSJ6vJ2LPxwZqnaNO6BqrfIPNQrRDEfQM4rSJFaUW0b8FdTy",
                Role = Role.Admin
            });
        });
    }
}
