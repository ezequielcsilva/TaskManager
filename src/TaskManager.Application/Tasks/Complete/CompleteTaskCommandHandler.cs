using FluentValidation;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Complete;

internal sealed class CompleteTaskCommandHandler(
    ITaskRepository taskRepository,
    IDbContext dbContext,
    IValidator<CompleteTaskCommand> validator) : ICommandHandler<CompleteTaskCommand, CompleteTaskResult>
{
    public async Task<Result<CompleteTaskResult>> Handle(CompleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorCode, e.ErrorMessage))
                .ToArray();

            return Result.Failure<CompleteTaskResult>(errors);
        }

        var task = await taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Failure<CompleteTaskResult>(TaskItemErrors.NotFound);
        }

        if (task.IsCompleted)
        {
            return Result.Failure<CompleteTaskResult>(TaskItemErrors.AlreadyCompleted);
        }

        task.Complete();
        taskRepository.Update(task);

        await dbContext.SaveChangesAsync(cancellationToken);

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