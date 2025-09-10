namespace TaskManager.Api.Models;

/// <summary>
/// Request for task creation
/// </summary>
public sealed record CreateTaskRequest
{
    /// <summary>
    /// Task title
    /// </summary>
    /// <example>Implement authentication</example>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Task description
    /// </summary>
    /// <example>Implement JWT authentication system for the API</example>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Task due date
    /// </summary>
    /// <example>2024-12-31T23:59:59Z</example>
    public DateTime DueDate { get; init; }

    /// <summary>
    /// Task owner user ID
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    public Guid UserId { get; init; }
}
