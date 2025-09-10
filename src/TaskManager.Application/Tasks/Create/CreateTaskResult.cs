namespace TaskManager.Application.Tasks.Create;

public sealed record CreateTaskResult(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime DueDate,
    Guid UserId,
    bool IsCompleted
);