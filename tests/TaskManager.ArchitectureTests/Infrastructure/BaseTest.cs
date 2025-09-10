using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;
using TaskManager.Application.Abstractions.Messaging;
using TaskManager.Domain.Abstractions;
using TaskManager.Infrastructure;

namespace TaskManager.ArchitectureTests.Infrastructure;

public abstract class BaseTest
{
    protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;

    protected static readonly Assembly DomainAssembly = typeof(Entity).Assembly;

    protected static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;

    protected static readonly Assembly PresentationApiAssembly = typeof(Program).Assembly;
}