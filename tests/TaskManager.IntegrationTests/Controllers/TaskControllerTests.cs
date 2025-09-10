using Bogus;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Api.Models;
using TaskManager.Application.Tasks.Complete;
using TaskManager.Application.Tasks.Create;
using TaskManager.Application.Tasks.GetTasksByUser;

namespace TaskManager.IntegrationTests.Controllers;

public class TaskControllerTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task GivenValidTask_WhenCreateTask_ThenTaskIsCreated()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = _faker.Lorem.Sentence(3),
            Description = _faker.Lorem.Paragraph(),
            DueDate = DateTime.UtcNow.AddDays(5),
            UserId = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateTaskResult>();
        result.Should().NotBeNull();
        result!.Title.Should().Be(request.Title);
        result.Description.Should().Be(request.Description);
        result.UserId.Should().Be(request.UserId);
        result.IsCompleted.Should().BeFalse();

        var dbTask = DbContext.Tasks.FirstOrDefault(t => t.Id == result.Id);
        dbTask.Should().NotBeNull();
        dbTask!.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task GivenExistingUser_WhenGetTasksByUser_ThenReturnsUserTasks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTaskRequest
        {
            Title = _faker.Lorem.Sentence(3),
            Description = _faker.Lorem.Paragraph(),
            DueDate = DateTime.UtcNow.AddDays(2),
            UserId = userId
        };
        await Client.PostAsJsonAsync("/api/tasks", request);

        // Act
        var response = await Client.GetAsync($"/api/tasks/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetTasksByUserListResult>();
        result.Should().NotBeNull();
        result!.Tasks.Should().ContainSingle(t => t.Title == request.Title && t.UserId == userId);
    }

    [Fact]
    public async Task GivenExistingTask_WhenCompleteTask_ThenTaskIsMarkedAsCompleted()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = _faker.Lorem.Sentence(3),
            Description = _faker.Lorem.Paragraph(),
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = Guid.NewGuid()
        };
        var createResponse = await Client.PostAsJsonAsync("/api/tasks", request);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResult>();

        // Act
        var response = await Client.PutAsync($"/api/tasks/{created!.Id}/complete", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CompleteTaskResult>();
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();

        var dbTask = DbContext.Tasks.FirstOrDefault(t => t.Id == created.Id);
        dbTask.Should().NotBeNull();
        dbTask!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task GivenExistingTask_WhenDeleteTask_ThenTaskIsDeleted()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            Title = _faker.Lorem.Sentence(3),
            Description = _faker.Lorem.Paragraph(),
            DueDate = DateTime.UtcNow.AddDays(3),
            UserId = Guid.NewGuid()
        };
        var createResponse = await Client.PostAsJsonAsync("/api/tasks", request);
        var created = await createResponse.Content.ReadFromJsonAsync<CreateTaskResult>();

        // Act
        var response = await Client.DeleteAsync($"/api/tasks/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var dbTask = DbContext.Tasks.FirstOrDefault(t => t.Id == created.Id);
        dbTask.Should().BeNull();
    }
}