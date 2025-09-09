using Bogus;
using FluentAssertions;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests.Domain.Tasks;

public class TaskItemTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Given_ValidParameters_When_CreateIsCalled_Then_TaskItemIsCreatedWithExpectedValues()
    {
        // Given
        var title = _faker.Lorem.Sentence();
        var description = _faker.Lorem.Paragraph();
        var dueDate = _faker.Date.Future();
        var userId = Guid.NewGuid();

        // When
        var taskItem = TaskItem.Create(title, description, dueDate, userId);

        // Then
        taskItem.Title.Should().Be(title);
        taskItem.Description.Should().Be(description);
        taskItem.DueDate.Should().Be(dueDate);
        taskItem.UserId.Should().Be(userId);
        taskItem.IsCompleted.Should().BeFalse();
        taskItem.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        taskItem.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Given_TaskItem_When_CompleteIsCalled_Then_IsCompletedShouldBeTrue()
    {
        // Given
        var taskItem = TaskItem.Create(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            _faker.Date.Future(),
            Guid.NewGuid()
        );

        // When
        taskItem.Complete();

        // Then
        taskItem.IsCompleted.Should().BeTrue();
    }
}