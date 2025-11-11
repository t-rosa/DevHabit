using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace DevHabit.Api.Modules.Tags.DTOs;

public sealed record CreateTagRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public sealed class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Description).MaximumLength(50);
    }
}
