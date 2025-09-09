using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Delete;

internal sealed class DeleteTaskCommandHandler(
    ITaskRepository taskRepository,
    IDbContext dbContext,
    IValidator<DeleteTaskCommand> validator,
    ILogger<DeleteTaskCommandHandler> logger) : ICommandHandler<DeleteTaskCommand, DeleteTaskResult>
{
    public async Task<Result<DeleteTaskResult>> Handle(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<DeleteTaskResult>(TaskItemErrors.InvalidRequest);
        }

        var task = await taskRepository.GetTasksByUserIdAsync(command.TaskId, cancellationToken);
        if (task is null)
        {
            logger.LogWarning("Task not found. TaskId: {TaskId}", command.TaskId);
            return Result.Failure<DeleteTaskResult>(TaskItemErrors.NotFound);
        }

        taskRepository.Delete(task);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Task deleted successfully. TaskId: {TaskId}", task.Id);

        var result = new DeleteTaskResult(task.Id);

        return Result.Success(result);
    }
}