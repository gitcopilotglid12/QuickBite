using QuickBite.AI.App.API.Data;
using QuickBite.AI.App.API.Models.Entities;
using QuickBite.AI.App.API.Models.Enums;

namespace QuickBite.AI.App.API.Services;

public interface IDataSeedingService
{
    Task SeedDataAsync();
}

public class DataSeedingService : IDataSeedingService
{
    private readonly QuickBiteDbContext _context;

    public DataSeedingService(QuickBiteDbContext context)
    {
        _context = context;
    }

    public async Task SeedDataAsync()
    {
        // Check if data already exists
        if (_context.FoodItems.Any())
        {
            return; // Data already seeded
        }

        var seedData = new List<FoodItem>
        {
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Classic Margherita Pizza",
                Description = "Traditional Italian pizza with fresh mozzarella, tomato sauce, and basil leaves",
                Price = 16.99m,
                Category = FoodCategory.MainCourses,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Grilled Chicken Caesar Salad",
                Description = "Fresh romaine lettuce with grilled chicken breast, parmesan cheese, and caesar dressing",
                Price = 14.50m,
                Category = FoodCategory.Salads,
                DietaryTag = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Spicy Buffalo Wings",
                Description = "Crispy chicken wings tossed in spicy buffalo sauce, served with celery and blue cheese",
                Price = 12.99m,
                Category = FoodCategory.Appetizers,
                DietaryTag = DietaryTag.Spicy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Creamy Tomato Basil Soup",
                Description = "Rich and creamy tomato soup with fresh basil, served with garlic bread",
                Price = 8.99m,
                Category = FoodCategory.Soups,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Chocolate Lava Cake",
                Description = "Warm chocolate cake with molten chocolate center, served with vanilla ice cream",
                Price = 9.99m,
                Category = FoodCategory.Desserts,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Beef Burger Deluxe",
                Description = "Juicy beef patty with lettuce, tomato, cheese, pickles, and special sauce on brioche bun",
                Price = 18.99m,
                Category = FoodCategory.MainCourses,
                DietaryTag = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Quinoa Buddha Bowl",
                Description = "Nutritious bowl with quinoa, roasted vegetables, avocado, and tahini dressing",
                Price = 15.99m,
                Category = FoodCategory.Salads,
                DietaryTag = DietaryTag.Vegan,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Garlic Parmesan Breadsticks",
                Description = "Warm breadsticks brushed with garlic butter and topped with parmesan cheese",
                Price = 7.99m,
                Category = FoodCategory.Appetizers,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Thai Green Curry",
                Description = "Authentic Thai green curry with coconut milk, vegetables, and jasmine rice",
                Price = 17.50m,
                Category = FoodCategory.MainCourses,
                DietaryTag = DietaryTag.Spicy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "New York Style Cheesecake",
                Description = "Rich and creamy cheesecake with graham cracker crust and berry compote",
                Price = 8.50m,
                Category = FoodCategory.Desserts,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "Mediterranean Hummus Platter",
                Description = "Creamy hummus served with pita bread, olives, cucumber, and cherry tomatoes",
                Price = 11.99m,
                Category = FoodCategory.Appetizers,
                DietaryTag = DietaryTag.Vegan,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new FoodItem
            {
                Id = Guid.NewGuid(),
                Name = "French Onion Soup",
                Description = "Classic French onion soup with caramelized onions, topped with melted cheese",
                Price = 10.99m,
                Category = FoodCategory.Soups,
                DietaryTag = DietaryTag.Vegetarian,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _context.FoodItems.AddRangeAsync(seedData);
        await _context.SaveChangesAsync();
    }
}