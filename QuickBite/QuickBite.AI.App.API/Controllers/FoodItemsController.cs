using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Services;

namespace QuickBite.AI.App.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodItemsController : ControllerBase
{
    private readonly IFoodItemService _foodItemService;
    private readonly IValidator<CreateFoodItemDto> _createValidator;
    private readonly IValidator<UpdateFoodItemDto> _updateValidator;

    public FoodItemsController(
        IFoodItemService foodItemService,
        IValidator<CreateFoodItemDto> createValidator,
        IValidator<UpdateFoodItemDto> updateValidator)
    {
        _foodItemService = foodItemService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodItemDto>>> GetAll()
    {
        var foodItems = await _foodItemService.GetAllAsync();
        return Ok(foodItems);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FoodItemDto>> GetById(Guid id)
    {
        var foodItem = await _foodItemService.GetByIdAsync(id);
        if (foodItem == null)
            return NotFound();

        return Ok(foodItem);
    }

    [HttpPost]
    public async Task<ActionResult<FoodItemDto>> Create(CreateFoodItemDto createDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var foodItem = await _foodItemService.CreateAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = foodItem.Id }, foodItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<FoodItemDto>> Update(Guid id, UpdateFoodItemDto updateDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var foodItem = await _foodItemService.UpdateAsync(id, updateDto);
        if (foodItem == null)
            return NotFound();

        return Ok(foodItem);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _foodItemService.DeleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}