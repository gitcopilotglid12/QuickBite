using FluentValidation;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Enums;

namespace QuickBite.AI.App.API.Validators;

public class UpdateFoodItemValidator : AbstractValidator<UpdateFoodItemDto>
{
    public UpdateFoodItemValidator()
    {
        // Name must be valid if provided - null or empty are not allowed
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name is required")
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(1, 100)
            .WithMessage("Name cannot exceed 100 characters")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be empty or whitespace");

        // Description can be null or empty - only validate length when provided
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid food category")
            .When(x => x.Category.HasValue);
    }
}