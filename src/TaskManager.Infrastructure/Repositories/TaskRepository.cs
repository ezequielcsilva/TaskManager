using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Tasks;

namespace TaskManager.Infrastructure.Repositories;

internal sealed class TaskRepository(ApplicationDbContext dbContext) : ITaskRepository
{
    public void Add(TaskItem task)
    {
        dbContext.Tasks.Add(task);
    }

    public void Delete(TaskItem task)
    {
        dbContext.Tasks.Remove(task);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToArrayAsync(cancellationToken);
    }

    public void Update(TaskItem task)
    {
        dbContext.Tasks.Update(task);
    }
}