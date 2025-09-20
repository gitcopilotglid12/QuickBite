using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuickBite.AI.App.API.Data;
using QuickBite.AI.App.API.Models.DTOs;
using QuickBite.AI.App.API.Models.Enums;
using Xunit;

namespace QuickBite.AI.App.Tests.Integration;

public class FoodItemsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public FoodItemsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<QuickBiteDbContext>));
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
                    options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllFoodItems_WhenNoItems_ShouldReturnEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/fooditems");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<FoodItemDto[]>(content, GetJsonOptions());
        items.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task CreateFoodItem_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateFoodItemDto
        {
            Name = "Test Pizza",
            Description = "A delicious test pizza",
            Price = 15.99m,
            Category = FoodCategory.MainCourses,
            DietaryTag = DietaryTag.Vegetarian
        };

        var json = JsonSerializer.Serialize(createDto, GetJsonOptions());
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/fooditems", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseContent = await response.Content.ReadAsStringAsync();
        var createdItem = JsonSerializer.Deserialize<FoodItemDto>(responseContent, GetJsonOptions());

        createdItem.Should().NotBeNull();
        createdItem!.Id.Should().NotBe(Guid.Empty);
        createdItem.Name.Should().Be(createDto.Name);
        createdItem.Description.Should().Be(createDto.Description);
        createdItem.Price.Should().Be(createDto.Price);
        createdItem.Category.Should().Be(createDto.Category);
        createdItem.DietaryTag.Should().Be(createDto.DietaryTag);
    }

    private static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }
}