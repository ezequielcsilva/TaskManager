using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Complete;

internal sealed class CompleteTaskCommandHandler(
    ITaskRepository taskRepository,
    IDbContext dbContext,
    IValidator<CompleteTaskCommand> validator,
    ILogger<CompleteTaskCommandHandler> logger) : ICommandHandler<CompleteTaskCommand, CompleteTaskResult>
{
    public async Task<Result<CompleteTaskResult>> Handle(CompleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<CompleteTaskResult>(TaskItemErrors.InvalidRequest);
        }

        var task = await taskRepository.GetTasksByUserIdAsync(command.TaskId, cancellationToken);
        if (task is null)
        {
            logger.LogWarning("Task not found. TaskId: {TaskId}", command.TaskId);
            return Result.Failure<CompleteTaskResult>(TaskItemErrors.NotFound);
        }

        if (task.IsCompleted)
        {
            return Result.Failure<CompleteTaskResult>(TaskItemErrors.AlreadyCompleted);
        }

        task.Complete();
        taskRepository.Update(task);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Task completed successfully. TaskId: {TaskId}", task.Id);

        var result = new CompleteTaskResult(
            task.Id,
            task.Title,
            task.Description,
            task.CreatedAt,
            task.DueDate,
            task.UserId,
            task.IsCompleted
        );

        return Result.Success(result);
    }
}