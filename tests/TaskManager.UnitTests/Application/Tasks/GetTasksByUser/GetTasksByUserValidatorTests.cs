using FluentAssertions;
using TaskManager.Application.Tasks.GetTasksByUser;

namespace TaskManager.UnitTests.Application.Tasks.GetTasksByUser;

public class GetTasksByUserValidatorTests
{
    [Fact]
    public void Should_Fail_When_UserId_Is_Empty()
    {
        var validator = new GetTasksByUserValidator();
        var query = new GetTasksByUserQuery(Guid.Empty);

        var result = validator.Validate(query);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_Pass_When_UserId_Is_Valid()
    {
        var validator = new GetTasksByUserValidator();
        var query = new GetTasksByUserQuery(Guid.NewGuid());

        var result = validator.Validate(query);

        result.IsValid.Should().BeTrue();
    }
}