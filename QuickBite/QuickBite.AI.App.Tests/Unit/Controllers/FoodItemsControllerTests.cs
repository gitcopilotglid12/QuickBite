using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using QuickBite.AI.App.API.Controllers;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Services;
using QuickBite.AI.App.Tests.TestData;
using Xunit;

namespace QuickBite.AI.App.Tests.Unit.Controllers;

public class FoodItemsControllerTests
{
    private readonly Mock<IFoodItemService> _mockService;
    private readonly Mock<IValidator<CreateFoodItemDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateFoodItemDto>> _mockUpdateValidator;
    private readonly FoodItemsController _controller;

    public FoodItemsControllerTests()
    {
        _mockService = new Mock<IFoodItemService>();
        _mockCreateValidator = new Mock<IValidator<CreateFoodItemDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateFoodItemDto>>();
        _controller = new FoodItemsController(_mockService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_WhenServiceReturnsItems_ShouldReturnOkWithItems()
    {
        // Arrange
        var testItems = FoodItemTestData.GetMultipleFoodItems();
        var expectedDtos = testItems.Select(item => new FoodItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            Category = item.Category,
            DietaryTag = item.DietaryTag,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        });

        _mockService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(expectedDtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<FoodItemDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<FoodItemDto>>().Subject;
        returnedItems.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_WhenServiceReturnsEmptyList_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllAsync())
            .ReturnsAsync(new List<FoodItemDto>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<FoodItemDto>>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<FoodItemDto>>().Subject;
        returnedItems.Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenItemExists_ShouldReturnOkWithItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var testItem = FoodItemTestData.GetValidFoodItem(itemId);
        var expectedDto = new FoodItemDto
        {
            Id = testItem.Id,
            Name = testItem.Name,
            Description = testItem.Description,
            Price = testItem.Price,
            Category = testItem.Category,
            DietaryTag = testItem.DietaryTag,
            CreatedAt = testItem.CreatedAt,
            UpdatedAt = testItem.UpdatedAt
        };

        _mockService.Setup(s => s.GetByIdAsync(itemId))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _controller.GetById(itemId);

        // Assert
        result.Should().BeOfType<ActionResult<FoodItemDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeOfType<FoodItemDto>().Subject;
        returnedItem.Id.Should().Be(itemId);
        returnedItem.Name.Should().Be(testItem.Name);
    }

    [Fact]
    public async Task GetById_WhenItemDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockService.Setup(s => s.GetByIdAsync(nonExistentId))
            .ReturnsAsync((FoodItemDto?)null);

        // Act
        var result = await _controller.GetById(nonExistentId);

        // Assert
        result.Should().BeOfType<ActionResult<FoodItemDto>>();
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var createDto = FoodItemTestData.GetValidCreateDto();
        var createdItem = new FoodItemDto
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

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.CreateAsync(createDto))
            .ReturnsAsync(createdItem);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Should().BeOfType<ActionResult<FoodItemDto>>();
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;

        createdResult.ActionName.Should().Be(nameof(FoodItemsController.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(createdItem.Id);

        var returnedItem = createdResult.Value.Should().BeOfType<FoodItemDto>().Subject;
        returnedItem.Should().BeEquivalentTo(createdItem);
    }

    [Fact]
    public async Task Create_ShouldCallServiceCreateAsync()
    {
        // Arrange
        var createDto = FoodItemTestData.GetValidCreateDto();
        var createdItem = new FoodItemDto
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

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateFoodItemDto>()))
            .ReturnsAsync(createdItem);

        // Act
        await _controller.Create(createDto);

        // Assert
        _mockService.Verify(s => s.CreateAsync(createDto), Times.Once);
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_WhenItemExists_ShouldReturnOkWithUpdatedItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = FoodItemTestData.GetValidUpdateDto();
        var updatedItem = new FoodItemDto
        {
            Id = itemId,
            Name = updateDto.Name!,
            Description = updateDto.Description,
            Price = updateDto.Price!.Value,
            Category = updateDto.Category!.Value,
            DietaryTag = updateDto.DietaryTag,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.UpdateAsync(itemId, updateDto))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _controller.Update(itemId, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<FoodItemDto>>();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedItem = okResult.Value.Should().BeOfType<FoodItemDto>().Subject;
        returnedItem.Should().BeEquivalentTo(updatedItem);
    }

    [Fact]
    public async Task Update_WhenItemDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = FoodItemTestData.GetValidUpdateDto();

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.UpdateAsync(nonExistentId, updateDto))
            .ReturnsAsync((FoodItemDto?)null);

        // Act
        var result = await _controller.Update(nonExistentId, updateDto);

        // Assert
        result.Should().BeOfType<ActionResult<FoodItemDto>>();
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldCallServiceUpdateAsync()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = FoodItemTestData.GetValidUpdateDto();
        var updatedItem = new FoodItemDto
        {
            Id = itemId,
            Name = updateDto.Name!,
            Description = updateDto.Description,
            Price = updateDto.Price!.Value,
            Category = updateDto.Category!.Value,
            DietaryTag = updateDto.DietaryTag,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateFoodItemDto>()))
            .ReturnsAsync(updatedItem);

        // Act
        await _controller.Update(itemId, updateDto);

        // Assert
        _mockService.Verify(s => s.UpdateAsync(itemId, updateDto), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenItemExists_ShouldReturnNoContent()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(itemId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WhenItemDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(nonExistentId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(nonExistentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldCallServiceDeleteAsync()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        await _controller.Delete(itemId);

        // Assert
        _mockService.Verify(s => s.DeleteAsync(itemId), Times.Once);
    }

    #endregion
}