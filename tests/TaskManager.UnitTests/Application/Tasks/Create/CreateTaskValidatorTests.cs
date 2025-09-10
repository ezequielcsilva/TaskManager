using FluentAssertions;
using TaskManager.Application.Tasks.Create;

namespace TaskManager.UnitTests.Application.Tasks.Create;

public class CreateTaskValidatorTests
{
    [Fact]
    public void Should_Fail_When_Fields_Are_Invalid()
    {
        var validator = new CreateTaskValidator();
        var command = new CreateTaskCommand("", "", DateTime.MinValue, Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_Fields_Are_Valid()
    {
        var validator = new CreateTaskValidator();
        var command = new CreateTaskCommand(
            "Title",
            "Description",
            DateTime.UtcNow.AddDays(1),
            Guid.NewGuid()
        );

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }
}