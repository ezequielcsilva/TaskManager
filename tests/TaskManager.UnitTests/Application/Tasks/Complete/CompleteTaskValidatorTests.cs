using FluentAssertions;
using TaskManager.Application.Tasks.Complete;

namespace TaskManager.UnitTests.Application.Tasks.Complete;

public class CompleteTaskValidatorTests
{
    [Fact]
    public void Should_Fail_When_TaskId_Is_Empty()
    {
        var validator = new CompleteTaskValidator();
        var command = new CompleteTaskCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_TaskId_Is_Valid()
    {
        var validator = new CompleteTaskValidator();
        var command = new CompleteTaskCommand(Guid.NewGuid());

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}