using Microsoft.EntityFrameworkCore;
using QuickBite.AI.App.API.Data;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Entities;

namespace QuickBite.AI.App.API.Services;

public class FoodItemService : IFoodItemService
{
    private readonly QuickBiteDbContext _context;

    public FoodItemService(QuickBiteDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FoodItemDto>> GetAllAsync()
    {
        var foodItems = await _context.FoodItems.ToListAsync();
        return foodItems.Select(MapToDto);
    }

    public async Task<FoodItemDto?> GetByIdAsync(Guid id)
    {
        var foodItem = await _context.FoodItems.FindAsync(id);
        return foodItem == null ? null : MapToDto(foodItem);
    }

    public async Task<FoodItemDto> CreateAsync(CreateFoodItemDto createDto)
    {
        var foodItem = new FoodItem
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Category = createDto.Category,
            DietaryTag = createDto.DietaryTag,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.FoodItems.Add(foodItem);
        await _context.SaveChangesAsync();

        return MapToDto(foodItem);
    }

    public async Task<FoodItemDto?> UpdateAsync(Guid id, UpdateFoodItemDto updateDto)
    {
        var foodItem = await _context.FoodItems.FindAsync(id);
        if (foodItem == null)
            return null;

        if (!string.IsNullOrEmpty(updateDto.Name))
            foodItem.Name = updateDto.Name;

        if (!string.IsNullOrEmpty(updateDto.Description))
            foodItem.Description = updateDto.Description;

        if (updateDto.Price.HasValue)
            foodItem.Price = updateDto.Price.Value;

        if (updateDto.Category.HasValue)
            foodItem.Category = updateDto.Category.Value;

        if (updateDto.DietaryTag.HasValue)
            foodItem.DietaryTag = updateDto.DietaryTag.Value;

        foodItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(foodItem);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var foodItem = await _context.FoodItems.FindAsync(id);
        if (foodItem == null)
            return false;

        _context.FoodItems.Remove(foodItem);
        await _context.SaveChangesAsync();
        return true;
    }

    private static FoodItemDto MapToDto(FoodItem foodItem)
    {
        return new FoodItemDto
        {
            Id = foodItem.Id,
            Name = foodItem.Name,
            Description = foodItem.Description,
            Price = foodItem.Price,
            Category = foodItem.Category,
            DietaryTag = foodItem.DietaryTag,
            CreatedAt = foodItem.CreatedAt,
            UpdatedAt = foodItem.UpdatedAt
        };
    }
}