using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Data;
using TaskManager.Domain.Tasks;

namespace TaskManager.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options) : DbContext(options), IDbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(builder);
    }
}