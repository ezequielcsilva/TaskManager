using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Application.Tasks.Delete;

namespace TaskManager.Application.Tasks.Delete;

public sealed record DeleteTaskCommand(
    Guid TaskId
) : ICommand;
