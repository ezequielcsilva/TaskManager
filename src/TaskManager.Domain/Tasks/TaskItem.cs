using TaskManager.Domain.Abstractions;

namespace TaskManager.Domain.Tasks;

public sealed class TaskItem : Entity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsCompleted { get; private set; }

    private TaskItem()
    { }

    private TaskItem(Guid id, string title, string description, DateTime createdAt, DateTime dueDate, Guid userId, bool isCompleted) : base(id)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        DueDate = dueDate;
        UserId = userId;
        IsCompleted = isCompleted;
    }

    public static TaskItem Create(string title, string description, DateTime dueDate, Guid userId)
    {
        return new TaskItem(
            Guid.NewGuid(),
            title,
            description,
            DateTime.UtcNow,
            dueDate,
            userId,
            false
        );
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}