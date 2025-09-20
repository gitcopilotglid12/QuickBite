using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.AI.App.API.Data;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Enums;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Xunit;

namespace QuickBite.AI.App.Tests.Integration;

public class ValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ValidationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<QuickBiteDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Also remove the DbContext itself
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(QuickBiteDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<QuickBiteDbContext>(options =>
                {
                    options.UseInMemoryDatabase("ValidationTestDatabase_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateFoodItem_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = "", // Invalid: empty name
            Price = -10, // Invalid: negative price
            Description = new string('x', 1001), // Invalid: too long description
            Category = (FoodCategory)999 // Invalid: non-existent category
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(errorResponse);
        Assert.Equal(400, errorResponse.StatusCode);
        Assert.Equal("Validation failed", errorResponse.Message);
        Assert.NotNull(errorResponse.Details);
        Assert.True(errorResponse.Details.Count > 0);
    }

    [Fact]
    public async Task CreateFoodItem_WithNullName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = null!, // Invalid: null name
            Price = 10.99m,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodItem_WithWhitespaceName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = "   ", // Invalid: whitespace only
            Price = 10.99m,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodItem_WithZeroPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = "Test Item",
            Price = 0, // Invalid: zero price
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodItem_WithLongName_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = new string('x', 101), // Invalid: too long name
            Price = 10.99m,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateFoodItem_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateFoodItemDto
        {
            Name = "Updated Item",
            Price = 15.99m,
            Description = "Updated description",
            Category = FoodCategory.MainCourses
        };

        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/fooditems/{invalidId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFoodItem_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/fooditems/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteFoodItem_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/fooditems/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100.50)]
    public async Task CreateFoodItem_WithInvalidPrice_ShouldReturnBadRequest(decimal invalidPrice)
    {
        // Arrange
        var invalidFoodItem = new CreateFoodItemDto
        {
            Name = "Test Item",
            Price = invalidPrice,
            Description = "Test description",
            Category = FoodCategory.MainCourses
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", invalidFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateFoodItem_WithValidDataAtBoundaries_ShouldSucceed()
    {
        // Arrange - Test minimum valid values
        var validFoodItem = new CreateFoodItemDto
        {
            Name = "A", // Minimum length
            Price = 0.01m, // Minimum price
            Description = "A", // Minimum description
            Category = FoodCategory.Appetizers
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/fooditems", validFoodItem);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ValidationError>? Details { get; set; }
}

public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string? AttemptedValue { get; set; }
}
