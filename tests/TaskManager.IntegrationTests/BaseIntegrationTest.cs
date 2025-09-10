using Microsoft.Extensions.DependencyInjection;
using TaskManager.Infrastructure;

namespace TaskManager.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly ApplicationDbContext DbContext;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Client = factory.CreateClient();
    }

    public void Dispose()
    {
        _scope.Dispose();
        Client.Dispose();
    }
}