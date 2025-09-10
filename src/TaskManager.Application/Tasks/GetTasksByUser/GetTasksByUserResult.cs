namespace TaskManager.Application.Tasks.GetTasksByUser;

public sealed record GetTasksByUserResult(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAt,
    DateTime DueDate,
    Guid UserId,
    bool IsCompleted
);

public sealed record GetTasksByUserListResult(
    IEnumerable<GetTasksByUserResult> Tasks
);