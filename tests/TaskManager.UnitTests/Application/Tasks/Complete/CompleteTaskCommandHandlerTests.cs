using Bogus;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Tasks.Complete;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests.Application.Tasks.Complete;

public class CompleteTaskCommandHandlerTests
{
    private readonly Faker _faker = new();
    private readonly IValidator<CompleteTaskCommand> _validator;
    private readonly ITaskRepository _repo;
    private readonly IDbContext _dbContext;
    private readonly CompleteTaskCommandHandler _handler;

    public CompleteTaskCommandHandlerTests()
    {
        _validator = Substitute.For<IValidator<CompleteTaskCommand>>();
        _repo = Substitute.For<ITaskRepository>();
        _dbContext = Substitute.For<IDbContext>();
        _handler = new CompleteTaskCommandHandler(_repo, _dbContext, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenTaskIsCompleted()
    {
        // Given
        var command = new CompleteTaskCommand(Guid.NewGuid());
        var task = TaskItem.Create(_faker.Lorem.Sentence(), _faker.Lorem.Paragraph(), _faker.Date.Future(), Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _repo.GetByIdAsync(command.TaskId, Arg.Any<CancellationToken>()).Returns(task);

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeTrue();
        _repo.Received(1).Update(task);
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenTaskNotFound()
    {
        // Given
        var command = new CompleteTaskCommand(Guid.NewGuid());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        _repo.GetByIdAsync(command.TaskId, Arg.Any<CancellationToken>()).Returns((TaskItem?)null);

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(TaskItemErrors.NotFound);
    }
}