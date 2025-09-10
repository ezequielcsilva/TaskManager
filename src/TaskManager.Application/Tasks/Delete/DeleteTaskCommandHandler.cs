using FluentValidation;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Delete;

internal sealed class DeleteTaskCommandHandler(
    ITaskRepository taskRepository,
    IDbContext dbContext,
    IValidator<DeleteTaskCommand> validator) : ICommandHandler<DeleteTaskCommand, DeleteTaskResult>
{
    public async Task<Result<DeleteTaskResult>> Handle(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<DeleteTaskResult>(TaskItemErrors.InvalidRequest);
        }

        var task = await taskRepository.GetByIdAsync(command.TaskId, cancellationToken);
        if (task is null)
        {
            return Result.Failure<DeleteTaskResult>(TaskItemErrors.NotFound);
        }

        taskRepository.Delete(task);

        await dbContext.SaveChangesAsync(cancellationToken);

        var result = new DeleteTaskResult(task.Id);

        return Result.Success(result);
    }
}