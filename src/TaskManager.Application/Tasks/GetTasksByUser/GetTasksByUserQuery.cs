using TaskManager.Application.Abstractions.Messaging;

namespace TaskManager.Application.Tasks.GetTasksByUser;

public sealed record GetTasksByUserQuery(
    Guid UserId
) : IQuery;