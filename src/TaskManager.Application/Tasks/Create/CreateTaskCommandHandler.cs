using FluentValidation;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.Create;

internal sealed class CreateTaskCommandHandler(
    ITaskRepository taskRepository,
    IDbContext dbContext,
    IValidator<CreateTaskCommand> validator) : ICommandHandler<CreateTaskCommand, CreateTaskResult>
{
    public async Task<Result<CreateTaskResult>> Handle(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<CreateTaskResult>(TaskItemErrors.InvalidRequest);
        }

        var task = TaskItem.Create(
            command.Title,
            command.Description,
            command.DueDate,
            command.UserId
        );

        taskRepository.Add(task);

        await dbContext.SaveChangesAsync(cancellationToken);

        var result = new CreateTaskResult(
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