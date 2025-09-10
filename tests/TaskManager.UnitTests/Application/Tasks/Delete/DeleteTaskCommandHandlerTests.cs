using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Tasks.Delete;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests.Application.Tasks.Delete;

public class DeleteTaskCommandHandlerTests
{
    private readonly IValidator<DeleteTaskCommand> _validator;
    private readonly ITaskRepository _repo;
    private readonly IDbContext _dbContext;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _validator = Substitute.For<IValidator<DeleteTaskCommand>>();
        _repo = Substitute.For<ITaskRepository>();
        _dbContext = Substitute.For<IDbContext>();
        _handler = new DeleteTaskCommandHandler(_repo, _dbContext, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTaskIsDeleted()
    {
        // Given
        var command = new DeleteTaskCommand(Guid.NewGuid());
        var task = TaskItem.Create("Title", "Description", DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _repo.GetByIdAsync(command.TaskId, Arg.Any<CancellationToken>()).Returns(task);

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeTrue();
        _repo.Received(1).Delete(task);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTaskNotFound()
    {
        // Given
        var command = new DeleteTaskCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _repo.GetByIdAsync(command.TaskId, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(TaskItemErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Given
        var command = new DeleteTaskCommand(Guid.Empty);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(
            [
                new FluentValidation.Results.ValidationFailure("TaskId", "Required")
            ]));

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}