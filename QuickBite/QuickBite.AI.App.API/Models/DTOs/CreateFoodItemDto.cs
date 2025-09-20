using QuickBite.AI.App.API.Models.Enums;

namespace QuickBite.AI.App.API.Models.DTOs;

public class CreateFoodItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public FoodCategory Category { get; set; }
    public DietaryTag? DietaryTag { get; set; }
}