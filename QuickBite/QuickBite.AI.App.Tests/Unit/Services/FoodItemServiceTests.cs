using FluentAssertions;
using QuickBite.AI.App.API.Services;
using QuickBite.AI.App.Tests.TestData;
using Xunit;

namespace QuickBite.AI.App.Tests.Unit.Services;

public class FoodItemServiceTests : IDisposable
{
    private readonly FoodItemService _service;
    private readonly AI.App.API.Data.QuickBiteDbContext _context;

    public FoodItemServiceTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _service = new FoodItemService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenNoFoodItems_ShouldReturnEmptyList()
    {
        // Arrange - empty database (already set up in constructor)

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenFoodItemsExist_ShouldReturnAllFoodItems()
    {
        // Arrange
        var testItems = FoodItemTestData.GetMultipleFoodItems();
        _context.FoodItems.AddRange(testItems);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        var resultList = result.ToList();
        resultList[0].Name.Should().Be("Caesar Salad");
        resultList[1].Name.Should().Be("Grilled Chicken");
        resultList[2].Name.Should().Be("Chocolate Cake");
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenFoodItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenFoodItemExists_ShouldReturnCorrectFoodItem()
    {
        // Arrange
        var testItem = FoodItemTestData.GetValidFoodItem();
        _context.FoodItems.Add(testItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetByIdAsync(testItem.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(testItem.Id);
        result.Name.Should().Be(testItem.Name);
        result.Description.Should().Be(testItem.Description);
        result.Price.Should().Be(testItem.Price);
        result.Category.Should().Be(testItem.Category);
        result.DietaryTag.Should().Be(testItem.DietaryTag);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAndReturnFoodItem()
    {
        // Arrange
        var createDto = FoodItemTestData.GetValidCreateDto();

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(createDto.Name);
        result.Description.Should().Be(createDto.Description);
        result.Price.Should().Be(createDto.Price);
        result.Category.Should().Be(createDto.Category);
        result.DietaryTag.Should().Be(createDto.DietaryTag);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        // Verify it was saved to database
        var savedItem = await _context.FoodItems.FindAsync(result.Id);
        savedItem.Should().NotBeNull();
        savedItem!.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var createDto = FoodItemTestData.GetValidCreateDto();
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        var afterCreate = DateTime.UtcNow;
        result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        result.CreatedAt.Should().BeOnOrBefore(afterCreate);
        result.UpdatedAt.Should().BeOnOrAfter(beforeCreate);
        result.UpdatedAt.Should().BeOnOrBefore(afterCreate);
        result.CreatedAt.Should().BeCloseTo(result.UpdatedAt, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenFoodItemDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = FoodItemTestData.GetValidUpdateDto();

        // Act
        var result = await _service.UpdateAsync(nonExistentId, updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateAndReturnFoodItem()
    {
        // Arrange
        var existingItem = FoodItemTestData.GetValidFoodItem();
        _context.FoodItems.Add(existingItem);
        await _context.SaveChangesAsync();

        var updateDto = FoodItemTestData.GetValidUpdateDto();
        var originalCreatedAt = existingItem.CreatedAt;

        // Act
        var result = await _service.UpdateAsync(existingItem.Id, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingItem.Id);
        result.Name.Should().Be(updateDto.Name);
        result.Description.Should().Be(updateDto.Description);
        result.Price.Should().Be(updateDto.Price);
        result.Category.Should().Be(updateDto.Category);
        result.DietaryTag.Should().Be(updateDto.DietaryTag);
        result.CreatedAt.Should().Be(originalCreatedAt); // Should not change
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_WithPartialData_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var existingItem = FoodItemTestData.GetValidFoodItem();
        _context.FoodItems.Add(existingItem);
        await _context.SaveChangesAsync();

        var partialUpdateDto = new API.Models.DTOs.UpdateFoodItemDto
        {
            Name = "Updated Name Only",
            // Other fields are null, should not be updated
        };

        // Act
        var result = await _service.UpdateAsync(existingItem.Id, partialUpdateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name Only");
        result.Description.Should().Be(existingItem.Description); // Unchanged
        result.Price.Should().Be(existingItem.Price); // Unchanged
        result.Category.Should().Be(existingItem.Category); // Unchanged
        result.DietaryTag.Should().Be(existingItem.DietaryTag); // Unchanged
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenFoodItemDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WhenFoodItemExists_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var existingItem = FoodItemTestData.GetValidFoodItem();
        _context.FoodItems.Add(existingItem);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteAsync(existingItem.Id);

        // Assert
        result.Should().BeTrue();

        // Verify it was deleted from database
        var deletedItem = await _context.FoodItems.FindAsync(existingItem.Id);
        deletedItem.Should().BeNull();
    }

    #endregion
}