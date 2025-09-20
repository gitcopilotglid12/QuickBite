using QuickBite.AI.App.API.Models.DTOs;

namespace QuickBite.AI.App.API.Services;

public interface IFoodItemService
{
    Task<IEnumerable<FoodItemDto>> GetAllAsync();
    Task<FoodItemDto?> GetByIdAsync(Guid id);
    Task<FoodItemDto> CreateAsync(CreateFoodItemDto createDto);
    Task<FoodItemDto?> UpdateAsync(Guid id, UpdateFoodItemDto updateDto);
    Task<bool> DeleteAsync(Guid id);
}