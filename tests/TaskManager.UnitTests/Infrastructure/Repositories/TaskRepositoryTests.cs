using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Tasks;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.UnitTests.Infrastructure.Repositories;

public class TaskRepositoryTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new ApplicationDbContext(options);
        _repository = new TaskRepository(_dbContext);
    }

    [Fact]
    public void Add_ShouldAddTask()
    {
        // Arrange
        var task = TaskItem.Create("Title", "Description", DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        // Act
        _repository.Add(task);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.Set<TaskItem>().Should().ContainEquivalentOf(task, options => options.Excluding(x => x.CreatedAt));
    }

    [Fact]
    public void Delete_ShouldRemoveTask()
    {
        // Arrange
        var task = TaskItem.Create("Title", "Description", DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        _dbContext.Set<TaskItem>().Add(task);
        _dbContext.SaveChanges();

        // Act
        _repository.Delete(task);
        _dbContext.SaveChanges();

        // Assert
        _dbContext.Set<TaskItem>().Should().NotContain(task);
    }

    [Fact]
    public void Update_ShouldUpdateTask()
    {
        // Arrange
        var task = TaskItem.Create("Title", "Description", DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        _dbContext.Set<TaskItem>().Add(task);
        _dbContext.SaveChanges();

        // Act
        task.Complete();
        _repository.Update(task);
        _dbContext.SaveChanges();

        // Assert
        var updated = _dbContext.Set<TaskItem>().Find(task.Id);
        updated!.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
    {
        // Arrange
        var task = TaskItem.Create("Title", "Description", DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        _dbContext.Set<TaskItem>().Add(task);
        _dbContext.SaveChanges();

        // Act
        var result = await _repository.GetByIdAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTasksByUserIdAsync_ShouldReturnTasksForUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var task1 = TaskItem.Create("Title1", "Desc1", DateTime.UtcNow.AddDays(1), userId);
        var task2 = TaskItem.Create("Title2", "Desc2", DateTime.UtcNow.AddDays(2), userId);
        var otherTask = TaskItem.Create("Other", "Other", DateTime.UtcNow.AddDays(3), Guid.NewGuid());
        _dbContext.Set<TaskItem>().AddRange(task1, task2, otherTask);
        _dbContext.SaveChanges();

        // Act
        var result = await _repository.GetTasksByUserIdAsync(userId);

        // Assert
        result.Should().HaveCount(2).And.OnlyContain(t => t.UserId == userId);
    }
}