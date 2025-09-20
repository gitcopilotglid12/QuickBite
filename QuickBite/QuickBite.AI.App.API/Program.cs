using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuickBite.AI.App.API.Data;
using QuickBite.AI.App.API.Middleware;
using QuickBite.AI.App.API.Services;
using QuickBite.AI.App.API.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateFoodItemValidator>();

// Add DbContext - only in non-test environments
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<QuickBiteDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=quickbite.db"));
}

// Add Services
builder.Services.AddScoped<IFoodItemService, FoodItemService>();
builder.Services.AddScoped<IDataSeedingService, DataSeedingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Add exception middleware
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seed data (only in non-test environments)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<QuickBiteDbContext>();
        context.Database.EnsureCreated();

        // Seed initial data
        var seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
        await seedingService.SeedDataAsync();
    }
}

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
