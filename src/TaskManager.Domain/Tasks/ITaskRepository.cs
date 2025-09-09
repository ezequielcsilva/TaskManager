namespace TaskManager.Domain.Tasks;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(TaskItem task);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}