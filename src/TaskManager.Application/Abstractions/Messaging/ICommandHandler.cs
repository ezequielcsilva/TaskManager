using TaskManager.Domain.Abstractions;

namespace TaskManager.Application.Abstractions.Messaging;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken = default);
}