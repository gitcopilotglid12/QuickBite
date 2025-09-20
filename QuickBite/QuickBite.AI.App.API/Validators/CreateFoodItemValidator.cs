using FluentValidation;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Enums;

namespace QuickBite.AI.App.API.Validators;

public class CreateFoodItemValidator : AbstractValidator<CreateFoodItemDto>
{
    public CreateFoodItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 100)
            .WithMessage("Name cannot exceed 100 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be empty or whitespace");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid food category");
    }
}