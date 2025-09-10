using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;
using TaskManager.ArchitectureTests.Infrastructure;
using TaskManager.Domain.Abstractions;

namespace TaskManager.ArchitectureTests.Domain;

public class DomainTests : BaseTest
{
    [Fact]
    public void Entities_ShouldHave_PrivateParameterlessConstructor()
    {
        IEnumerable<Type> entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var failingTypes = (
            from entityType in entityTypes
            let constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            where !constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0)
            select entityType).ToList();

        failingTypes.Should().BeEmpty();
    }
}