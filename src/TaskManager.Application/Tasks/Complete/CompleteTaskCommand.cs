using TaskManager.Application.Abstractions.Messaging;

namespace TaskManager.Application.Tasks.Complete;

public sealed record CompleteTaskCommand(
    Guid TaskId
) : ICommand;