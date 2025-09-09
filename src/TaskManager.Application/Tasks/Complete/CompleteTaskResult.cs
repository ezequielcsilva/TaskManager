namespace TaskManager.Application.Tasks.Complete;

public sealed record CompleteTaskResult(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime DueDate,
    Guid UserId,
    bool IsCompleted
);
