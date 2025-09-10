using FluentValidation;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Domain.Tasks;

namespace TaskManager.Application.Tasks.GetTasksByUser;

internal sealed class GetTasksByUserQueryHandler(
    ITaskRepository taskRepository,
    IValidator<GetTasksByUserQuery> validator) : IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult>
{
    public async Task<Result<GetTasksByUserListResult>> Handle(GetTasksByUserQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<GetTasksByUserListResult>(TaskItemErrors.InvalidRequest);
        }

        var tasks = await taskRepository.GetTasksByUserIdAsync(query.UserId, cancellationToken);

        if (tasks is null)
        {
            return Result.Failure<GetTasksByUserListResult>(TaskItemErrors.NotFound);
        }

        var result = new GetTasksByUserListResult(tasks
                                .Select(t => new GetTasksByUserResult(
                                    t.Id,
                                    t.Title,
                                    t.Description,
                                    t.CreatedAt,
                                    t.DueDate,
                                    t.UserId,
                                    t.IsCompleted
                                )));

        return Result.Success(result);
    }
}