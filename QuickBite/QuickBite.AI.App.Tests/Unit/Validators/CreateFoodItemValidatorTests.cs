using FluentValidation;
using FluentValidation.TestHelper;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Enums;
using QuickBite.AI.App.API.Validators;
using Xunit;

namespace QuickBite.AI.App.Tests.Unit.Validators;

public class CreateFoodItemValidatorTests
{
    private readonly CreateFoodItemValidator _validator;

    public CreateFoodItemValidatorTests()
    {
        _validator = new CreateFoodItemValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Name = "" };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name is required");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Null()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Name = null! };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Whitespace()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Name = "   " };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name cannot be empty or whitespace");
    }

    [Fact]
    public void Should_Have_Error_When_Name_Exceeds_MaxLength()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Name = new string('x', 101) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name cannot exceed 100 characters");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100.50)]
    public void Should_Have_Error_When_Price_Is_Invalid(decimal invalidPrice)
    {
        // Arrange
        var dto = new CreateFoodItemDto { Price = invalidPrice };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Price)
              .WithErrorMessage("Price must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_MaxLength()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Description = new string('x', 1001) };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description cannot exceed 1000 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Category_Is_Invalid()
    {
        // Arrange
        var dto = new CreateFoodItemDto { Category = (FoodCategory)999 };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Category)
              .WithErrorMessage("Invalid food category");
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
    {
        // Arrange
        var dto = new CreateFoodItemDto
        {
            Name = "Chicken Burger",
            Price = 12.99m,
            Description = "Delicious chicken burger",
            Category = FoodCategory.MainCourses
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Empty()
    {
        // Arrange
        var dto = new CreateFoodItemDto
        {
            Name = "Chicken Burger",
            Price = 12.99m,
            Description = "",
            Category = FoodCategory.MainCourses
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        // Arrange
        var dto = new CreateFoodItemDto
        {
            Name = "Chicken Burger",
            Price = 12.99m,
            Description = null,
            Category = FoodCategory.MainCourses
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1.00)]
    [InlineData(999.99)]
    public void Should_Not_Have_Error_When_Price_Is_Valid(decimal validPrice)
    {
        // Arrange
        var dto = new CreateFoodItemDto
        {
            Name = "Test Item",
            Price = validPrice,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Price);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Pizza")]
    [InlineData("Very Long Food Item Name That Is Still Under The Limit")]
    public void Should_Not_Have_Error_When_Name_Is_Valid(string validName)
    {
        // Arrange
        var dto = new CreateFoodItemDto
        {
            Name = validName,
            Price = 10.99m,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act & Assert
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}