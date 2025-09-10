using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TaskManager.Application.Tasks.GetTasksByUser;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests.Application.Tasks.GetTasksByUser;

public class GetTasksByUserQueryHandlerTests
{
    private readonly IValidator<GetTasksByUserQuery> _validator;
    private readonly ITaskRepository _repo;
    private readonly GetTasksByUserQueryHandler _handler;

    public GetTasksByUserQueryHandlerTests()
    {
        _validator = Substitute.For<IValidator<GetTasksByUserQuery>>();
        _repo = Substitute.For<ITaskRepository>();
        _handler = new GetTasksByUserQueryHandler(_repo, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTasksExist()
    {
        // Given
        var userId = Guid.NewGuid();
        var query = new GetTasksByUserQuery(userId);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        var tasks = new List<TaskItem>
        {
            TaskItem.Create("Title", "Desc", DateTime.UtcNow.AddDays(1), userId)
        };

        _repo.GetTasksByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(tasks.AsEnumerable());

        // When
        var result = await _handler.Handle(query);

        // Then
        result.IsSuccess.Should().BeTrue();
        result.Value.Tasks.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Given
        var query = new GetTasksByUserQuery(Guid.Empty);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(
            [
                new FluentValidation.Results.ValidationFailure("UserId", "Required")
            ]));

        // When
        var result = await _handler.Handle(query);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(TaskItemErrors.InvalidRequest);
    }
}