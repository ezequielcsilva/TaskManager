using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Application.Tasks.Create;
using TaskManager.Application.Tasks.Complete;
using TaskManager.Application.Tasks.Delete;
using TaskManager.Application.Tasks.GetTasksByUser;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddCommandsHandlers()
            .AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }

    private static IServiceCollection AddCommandsHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateTaskCommand, CreateTaskResult>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteTaskCommand, CompleteTaskResult>, CompleteTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand, DeleteTaskResult>, DeleteTaskCommandHandler>();
        services.AddScoped<IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult>, GetTasksByUserQueryHandler>();

        return services;
    }
}