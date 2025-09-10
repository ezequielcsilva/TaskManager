using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Application.Tasks.Complete;
using TaskManager.Application.Tasks.Create;
using TaskManager.Application.Tasks.Delete;
using TaskManager.Application.Tasks.GetTasksByUser;
using TaskManager.Domain.Abstractions;
using TaskManager.Api.Models;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ICommandHandler<CreateTaskCommand, CreateTaskResult> _createTaskHandler;
    private readonly IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult> _getTasksByUserHandler;
    private readonly ICommandHandler<CompleteTaskCommand, CompleteTaskResult> _completeTaskHandler;
    private readonly ICommandHandler<DeleteTaskCommand, DeleteTaskResult> _deleteTaskHandler;

    public TaskController(
        ICommandHandler<CreateTaskCommand, CreateTaskResult> createTaskHandler,
        IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult> getTasksByUserHandler,
        ICommandHandler<CompleteTaskCommand, CompleteTaskResult> completeTaskHandler,
        ICommandHandler<DeleteTaskCommand, DeleteTaskResult> deleteTaskHandler)
    {
        _createTaskHandler = createTaskHandler;
        _getTasksByUserHandler = getTasksByUserHandler;
        _completeTaskHandler = completeTaskHandler;
        _deleteTaskHandler = deleteTaskHandler;
    }

    /// <summary>
    /// Creates a new task
    /// </summary>
    /// <param name="request">Task data to be created</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created task data</returns>
    /// <response code="201">Task created successfully</response>
    /// <response code="400">Invalid data provided</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTaskResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateTaskCommand(
            request.Title,
            request.Description,
            request.DueDate,
            request.UserId);

        var result = await _createTaskHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtAction(nameof(GetTasksByUser), new { userId = result.Value.UserId }, result.Value);
    }

    /// <summary>
    /// Lists all tasks for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's task list</returns>
    /// <response code="200">Task list returned successfully</response>
    /// <response code="400">Invalid user ID</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(GetTasksByUserListResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTasksByUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTasksByUserQuery(userId);
        var result = await _getTasksByUserHandler.Handle(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Marks a task as completed
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated task data</returns>
    /// <response code="200">Task marked as completed successfully</response>
    /// <response code="400">Invalid task ID</response>
    /// <response code="404">Task not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(CompleteTaskResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteTask(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new CompleteTaskCommand(id);
        var result = await _completeTaskHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deletes a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="204">Task deleted successfully</response>
    /// <response code="400">Invalid task ID</response>
    /// <response code="404">Task not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTask(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteTaskCommand(id);
        var result = await _deleteTaskHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}