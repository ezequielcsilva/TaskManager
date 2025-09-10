using FluentValidation;

namespace TaskManager.Application.Tasks.Delete;

internal sealed class DeleteTaskValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("Task ID is required");
    }
}