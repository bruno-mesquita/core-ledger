using Testcontainers.PostgreSql;

namespace CoreLedger.IntegrationTests.Helpers;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("coreledger_test")
        .WithUsername("coreledger")
        .WithPassword("coreledger_test")
        .Build();

    protected HttpClient Client { get; private set; } = null!;
    protected CustomWebApplicationFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        Factory = new CustomWebApplicationFactory(_postgres.GetConnectionString());
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
