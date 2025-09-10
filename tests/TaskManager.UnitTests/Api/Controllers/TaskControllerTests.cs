using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TaskManager.Api.Controllers;
using TaskManager.Api.Models;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Application.Tasks.Complete;
using TaskManager.Application.Tasks.Create;
using TaskManager.Application.Tasks.Delete;
using TaskManager.Application.Tasks.GetTasksByUser;
using TaskManager.Domain.Abstractions;

namespace TaskManager.UnitTests.Api.Controllers;

public class TaskControllerTests
{
    private readonly Faker _faker = new();
    private readonly ICommandHandler<CreateTaskCommand, CreateTaskResult> _createHandler;
    private readonly IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult> _getTasksByUserHandler;
    private readonly ICommandHandler<CompleteTaskCommand, CompleteTaskResult> _completeHandler;
    private readonly ICommandHandler<DeleteTaskCommand, DeleteTaskResult> _deleteHandler;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _createHandler = Substitute.For<ICommandHandler<CreateTaskCommand, CreateTaskResult>>();
        _getTasksByUserHandler = Substitute.For<IQueryHandler<GetTasksByUserQuery, GetTasksByUserListResult>>();
        _completeHandler = Substitute.For<ICommandHandler<CompleteTaskCommand, CompleteTaskResult>>();
        _deleteHandler = Substitute.For<ICommandHandler<DeleteTaskCommand, DeleteTaskResult>>();
        _controller = new TaskController(_createHandler, _getTasksByUserHandler, _completeHandler, _deleteHandler);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreated_WhenTaskIsCreated()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            DueDate = _faker.Date.Future(),
            UserId = Guid.NewGuid()
        };
        var command = new CreateTaskCommand(request.Title, request.Description, request.DueDate, request.UserId);
        var result = Result.Success(new CreateTaskResult(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            request.DueDate,
            DateTime.UtcNow,
            request.UserId,
            true));

        _createHandler.Handle(Arg.Is<CreateTaskCommand>(c =>
            c.Title == request.Title &&
            c.Description == request.Description &&
            c.DueDate == request.DueDate &&
            c.UserId == request.UserId
        ), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.CreateTask(request);

        // Assert
        var createdResult = response as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(result.Value);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = "",
            Description = _faker.Lorem.Paragraph(),
            DueDate = _faker.Date.Future(),
            UserId = Guid.NewGuid()
        };
        var command = new CreateTaskCommand(request.Title, request.Description, request.DueDate, request.UserId);
        var error = new Error("Title", "Title is required");
        var result = Result.Failure<CreateTaskResult>(error);

        _createHandler.Handle(Arg.Any<CreateTaskCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.CreateTask(request);

        // Assert
        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(result.Errors);
    }

    [Fact]
    public async Task GetTasksByUser_ShouldReturnOk_WithTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetTasksByUserQuery(userId);
        var tasks = new List<GetTasksByUserResult>
        {
            new(Guid.NewGuid(),
                _faker.Lorem.Sentence(),
                _faker.Lorem.Paragraph(),
                _faker.Date.Future(),
                _faker.Date.Future(),
                userId,
                false)
        };
        var result = Result.Success(new GetTasksByUserListResult(tasks));

        _getTasksByUserHandler.Handle(Arg.Is<GetTasksByUserQuery>(q => q.UserId == userId), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.GetTasksByUser(userId);

        // Assert
        var okResult = response as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(result.Value);
    }

    [Fact]
    public async Task GetTasksByUser_ShouldReturnBadRequest_WhenQueryFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var error = new Error("UserId", "Invalid user id");
        var result = Result.Failure<GetTasksByUserListResult>(error);

        _getTasksByUserHandler.Handle(Arg.Any<GetTasksByUserQuery>(), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.GetTasksByUser(userId);

        // Assert
        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(result.Errors);
    }

    [Fact]
    public async Task CompleteTask_ShouldReturnOk_WhenTaskIsCompleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new CompleteTaskCommand(taskId);
        var result = Result.Success(new CompleteTaskResult
        (
            taskId,
            "",
            _faker.Lorem.Paragraph(),
            _faker.Date.Future(),
            _faker.Date.Future(),
            Guid.NewGuid(),
            true
        ));

        _completeHandler.Handle(Arg.Is<CompleteTaskCommand>(c => c.TaskId == taskId), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.CompleteTask(taskId);

        // Assert
        var okResult = response as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(result.Value);
    }

    [Fact]
    public async Task CompleteTask_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var error = new Error("TaskId", "Invalid task id");
        var result = Result.Failure<CompleteTaskResult>(error);

        _completeHandler.Handle(Arg.Any<CompleteTaskCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.CompleteTask(taskId);

        // Assert
        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(result.Errors);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNoContent_WhenTaskIsDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);
        var result = Result.Success(new DeleteTaskResult(taskId));

        _deleteHandler.Handle(Arg.Is<DeleteTaskCommand>(c => c.TaskId == taskId), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.DeleteTask(taskId);

        // Assert
        var noContent = response as NoContentResult;
        noContent.Should().NotBeNull();
        noContent!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var error = new Error("TaskId", "Invalid task id");
        var result = Result.Failure<DeleteTaskResult>(error);

        _deleteHandler.Handle(Arg.Any<DeleteTaskCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        // Act
        var response = await _controller.DeleteTask(taskId);

        // Assert
        var badRequest = response as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(result.Errors);
    }
}