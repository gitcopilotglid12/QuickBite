using QuickBite.AI.App.API.Models.Entities;
using QuickBite.AI.App.API.Models.Enums;
using QuickBite.AI.App.API.Models.DTOs;

namespace QuickBite.AI.App.Tests.TestData;

public static class FoodItemTestData
{
    public static FoodItem GetValidFoodItem(Guid? id = null)
    {
        return new FoodItem
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Margherita Pizza",
            Description = "Classic pizza with tomato sauce, mozzarella, and basil",
            Price = 12.99m,
            Category = FoodCategory.MainCourses,
            DietaryTag = DietaryTag.Vegetarian,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }

    public static List<FoodItem> GetMultipleFoodItems()
    {
        return new List<FoodItem>
        {
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Caesar Salad",
                Description = "Fresh romaine lettuce with caesar dressing",
                Price = 8.99m,
                Category = FoodCategory.Salads,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Grilled Chicken",
                Description = "Juicy grilled chicken breast with herbs",
                Price = 15.99m,
                Category = FoodCategory.MainCourses,
                DietaryTag = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Chocolate Cake",
                Description = "Rich chocolate cake with dark chocolate frosting",
                Price = 6.99m,
                Category = FoodCategory.Desserts,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    public static CreateFoodItemDto GetValidCreateDto()
    {
        return new CreateFoodItemDto
        {
            Name = "BBQ Burger",
            Description = "Beef burger with BBQ sauce and crispy onions",
            Price = 13.99m,
            Category = FoodCategory.MainCourses,
            DietaryTag = null
        };
    }

    public static UpdateFoodItemDto GetValidUpdateDto()
    {
        return new UpdateFoodItemDto
        {
            Name = "Updated Pizza Name",
            Description = "Updated description",
            Price = 14.99m,
            Category = FoodCategory.MainCourses,
            DietaryTag = DietaryTag.Vegan
        };
    }

    public static CreateFoodItemDto GetInvalidCreateDto()
    {
        return new CreateFoodItemDto
        {
            Name = "", // Invalid - empty name
            Description = "Valid description",
            Price = -5.99m, // Invalid - negative price
            Category = FoodCategory.MainCourses,
            DietaryTag = DietaryTag.Vegetarian
        };
    }
}