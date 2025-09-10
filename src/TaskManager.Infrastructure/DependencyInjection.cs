using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Domain.Tasks;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
   this IServiceCollection services,
   IConfiguration configuration)
    {
        AddPersistence(services, configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("TaskManagerDatabase");
        });
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }
}