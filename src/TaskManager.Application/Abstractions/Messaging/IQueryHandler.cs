using TaskManager.Domain.Abstractions;

namespace TaskManager.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken = default);
}