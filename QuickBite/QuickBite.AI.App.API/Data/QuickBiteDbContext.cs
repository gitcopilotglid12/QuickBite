using Microsoft.EntityFrameworkCore;
using QuickBite.AI.App.API.Models.Entities;

namespace QuickBite.AI.App.API.Data;

public class QuickBiteDbContext : DbContext
{
    public QuickBiteDbContext(DbContextOptions<QuickBiteDbContext> options) : base(options)
    {
    }

    public DbSet<FoodItem> FoodItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure FoodItem entity
        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We'll generate GUIDs in the service

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Category)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.DietaryTag)
                .HasConversion<string>();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });
    }
}