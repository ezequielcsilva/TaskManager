namespace TaskManager.Domain.Tasks;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(TaskItem task);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}