using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Models;
using ExpensiveTrackerAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpensiveTrackerAPI.Tests;

public class CategoryServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsOnlyUserCategories()
    {
        await using var context = CreateContext();
        var user1 = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        var user2 = new User { Name = "B", Email = "b@b.com", CreatedAt = DateTime.UtcNow, PasswordHash = "y" };
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();

        await context.Categories.AddRangeAsync(
            new Category { Name = "Income", Type = TransactionType.Income, UserId = user1.Id },
            new Category { Name = "Expense", Type = TransactionType.Expense, UserId = user1.Id },
            new Category { Name = "Other", Type = TransactionType.Expense, UserId = user2.Id }
        );
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var result = await service.GetCategoriesAsync(user1.Id);

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(user1.Id, c.UserId));
    }

    [Fact]
    public async Task GetCategoryAsync_WhenExists_ReturnsCategory()
    {
        await using var context = CreateContext();
        var user = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var category = new Category { Name = "Income", Type = TransactionType.Income, UserId = user.Id };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var result = await service.GetCategoryAsync(user.Id, category.Id);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result!.Id);
    }

    [Fact]
    public async Task GetCategoryAsync_WhenNotExists_ReturnsNull()
    {
        await using var context = CreateContext();
        var user = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var service = new CategoryService(context);
        var result = await service.GetCategoryAsync(user.Id, 999);

        Assert.Null(result);
    }
}
