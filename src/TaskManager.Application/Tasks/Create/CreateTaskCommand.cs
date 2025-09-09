using TaskManager.Application.Abstractions.Messaging;

namespace TaskManager.Application.Tasks.Create;

public sealed record CreateTaskCommand(
    string Title,
    string Description,
    DateTime DueDate,
    Guid UserId
) : ICommand;