using Microsoft.EntityFrameworkCore;
using QuickBite.AI.App.API.Data;

namespace QuickBite.AI.App.Tests.TestData;

public static class TestDbContextFactory
{
    public static QuickBiteDbContext CreateInMemoryContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<QuickBiteDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        var context = new QuickBiteDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public static QuickBiteDbContext CreateContextWithData(string databaseName = "")
    {
        var context = CreateInMemoryContext(databaseName);

        // Add test data
        var testItems = FoodItemTestData.GetMultipleFoodItems();
        context.FoodItems.AddRange(testItems);
        context.SaveChanges();

        // Detach entities to avoid tracking issues in tests
        foreach (var entity in context.ChangeTracker.Entries())
        {
            entity.State = EntityState.Detached;
        }

        return context;
    }
}