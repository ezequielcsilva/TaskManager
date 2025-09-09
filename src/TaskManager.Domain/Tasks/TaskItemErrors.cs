using TaskManager.Domain.Abstractions;

namespace TaskManager.Domain.Tasks;

public static class TaskItemErrors
{
    public static readonly Error InvalidRequest = new("TaskItem.InvalidRequest", "Request cannot be null");
    public static readonly Error InvalidTaskId = new("TaskItem.InvalidTaskId", "Task ID cannot be empty");
    public static readonly Error InvalidUserId = new("TaskItem.InvalidUserId", "User ID cannot be empty");
    public static readonly Error InvalidTitle = new("TaskItem.InvalidTitle", "Title cannot be null or empty");
    public static readonly Error InvalidDescription = new("TaskItem.InvalidDescription", "Description cannot be null or empty");
    public static readonly Error InvalidDueDate = new("TaskItem.InvalidDueDate", "Due date must be in the future");
    public static readonly Error NotFound = new("TaskItem.NotFound", "Task not found");
    public static readonly Error AlreadyCompleted = new("TaskItem.AlreadyCompleted", "Task is already completed");
    public static readonly Error NotCompleted = new("TaskItem.NotCompleted", "Task is not completed");
    public static readonly Error CreationFailed = new("TaskItem.CreationFailed", "Failed to create task");
    public static readonly Error UpdateFailed = new("TaskItem.UpdateFailed", "Failed to update task");
    public static readonly Error DeletionFailed = new("TaskItem.DeletionFailed", "Failed to delete task");
    public static readonly Error RetrievalFailed = new("TaskItem.RetrievalFailed", "Failed to retrieve tasks");
}
