using FluentValidation;

namespace TaskManager.Application.Tasks.GetTasksByUser;

internal sealed class GetTasksByUserValidator : AbstractValidator<GetTasksByUserQuery>
{
    public GetTasksByUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}