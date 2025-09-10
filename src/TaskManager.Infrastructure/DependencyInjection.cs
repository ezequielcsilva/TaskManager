using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TaskManager.Application.Abstractions.Data;
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
        
        // Registrar IDbContext para que os handlers possam injetá-lo
        services.AddScoped<IDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<ITaskRepository, TaskRepository>();
        return services;
    }
}