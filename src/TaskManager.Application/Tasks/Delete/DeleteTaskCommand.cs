using TaskManager.Application.Abstractions.Messaging;

namespace TaskManager.Application.Tasks.Delete;

public sealed record DeleteTaskCommand(
    Guid TaskId
) : ICommand;