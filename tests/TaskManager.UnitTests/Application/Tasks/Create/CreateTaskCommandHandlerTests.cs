using Bogus;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Application.Tasks.Create;
using TaskManager.Domain.Tasks;

namespace TaskManager.UnitTests.Application.Tasks.Create;

public class CreateTaskCommandHandlerTests
{
    private readonly Faker _faker = new();
    private readonly IValidator<CreateTaskCommand> _validator;
    private readonly ITaskRepository _repo;
    private readonly IDbContext _dbContext;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _validator = Substitute.For<IValidator<CreateTaskCommand>>();
        _repo = Substitute.For<ITaskRepository>();
        _dbContext = Substitute.For<IDbContext>();
        _handler = new CreateTaskCommandHandler(_repo, _dbContext, _validator);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Given
        var command = new CreateTaskCommand(
            _faker.Lorem.Sentence(),
            _faker.Lorem.Paragraph(),
            _faker.Date.Future(),
            Guid.NewGuid()
        );

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult());

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        _repo.Received(1).Add(Arg.Any<TaskItem>());
        await _dbContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Given
        var command = new CreateTaskCommand("", "", DateTime.MinValue, Guid.Empty);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new FluentValidation.Results.ValidationResult(
            [
                new FluentValidation.Results.ValidationFailure("Title", "Required")
            ]));

        // When
        var result = await _handler.Handle(command);

        // Then
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}