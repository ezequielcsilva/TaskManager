using FluentAssertions;
using TaskManager.Application.Tasks.Delete;

namespace TaskManager.UnitTests.Application.Tasks.Delete;

public class DeleteTaskValidatorTests
{
    [Fact]
    public void Should_Fail_When_TaskId_Is_Empty()
    {
        var validator = new DeleteTaskValidator();
        var command = new DeleteTaskCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_TaskId_Is_Valid()
    {
        var validator = new DeleteTaskValidator();
        var command = new DeleteTaskCommand(Guid.NewGuid());

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}