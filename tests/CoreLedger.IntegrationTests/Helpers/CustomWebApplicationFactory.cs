using CoreLedger.Infrastructure.Options;
using CoreLedger.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CoreLedger.IntegrationTests.Helpers;

public class CustomWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    private const string TestSecretKey = "test-secret-key-at-least-32-chars-long-coreledger!!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Jwt:SecretKey", TestSecretKey);
        builder.UseSetting("Jwt:Issuer", "coreledger");
        builder.UseSetting("Jwt:Audience", "coreledger-clients");
        builder.UseSetting("Jwt:AccessTokenExpirationMinutes", "15");
        builder.UseSetting("Jwt:RefreshTokenExpirationDays", "7");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Ensure JwtOptions are configured with test values
            services.Configure<JwtOptions>(opts =>
            {
                opts.SecretKey = TestSecretKey;
                opts.Issuer = "coreledger";
                opts.Audience = "coreledger-clients";
                opts.AccessTokenExpirationMinutes = 15;
                opts.RefreshTokenExpirationDays = 7;
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        return host;
    }
}
