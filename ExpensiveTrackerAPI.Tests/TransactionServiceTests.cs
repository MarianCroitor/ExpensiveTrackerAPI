using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Models;
using ExpensiveTrackerAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpensiveTrackerAPI.Tests;

public class TransactionServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateTransactionAsync_WhenCategoryExists_CreatesTransaction()
    {
        await using var context = CreateContext();
        var user = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var category = new Category { Name = "Income", Type = TransactionType.Income, UserId = user.Id };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var result = await service.CreateTransactionAsync(user.Id, category.Id, 120, "Salary", DateTime.UtcNow);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result!.CategoryId);
        Assert.Single(context.Transactions);
    }

    [Fact]
    public async Task CreateTransactionAsync_WhenCategoryNotOwned_ReturnsNull()
    {
        await using var context = CreateContext();
        var user1 = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        var user2 = new User { Name = "B", Email = "b@b.com", CreatedAt = DateTime.UtcNow, PasswordHash = "y" };
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();

        var category = new Category { Name = "Income", Type = TransactionType.Income, UserId = user1.Id };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var result = await service.CreateTransactionAsync(user2.Id, category.Id, 10, "Bad", DateTime.UtcNow);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsOnlyUserTransactions()
    {
        await using var context = CreateContext();
        var user1 = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        var user2 = new User { Name = "B", Email = "b@b.com", CreatedAt = DateTime.UtcNow, PasswordHash = "y" };
        await context.Users.AddRangeAsync(user1, user2);
        await context.SaveChangesAsync();

        var category1 = new Category { Name = "Income", Type = TransactionType.Income, UserId = user1.Id };
        var category2 = new Category { Name = "Expense", Type = TransactionType.Expense, UserId = user2.Id };
        await context.Categories.AddRangeAsync(category1, category2);
        await context.SaveChangesAsync();

        await context.Transactions.AddRangeAsync(
            new Transaction { Amount = 5, Description = "A", Date = DateTime.UtcNow, CategoryId = category1.Id },
            new Transaction { Amount = 7, Description = "B", Date = DateTime.UtcNow, CategoryId = category2.Id }
        );
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var result = await service.GetTransactionsAsync(user1.Id);

        Assert.Single(result);
        Assert.Equal(category1.Id, result[0].CategoryId);
    }

    [Fact]
    public async Task UpdateTransactionAsync_UpdatesAmountAndDescription()
    {
        await using var context = CreateContext();
        var user = new User { Name = "A", Email = "a@a.com", CreatedAt = DateTime.UtcNow, PasswordHash = "x" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        var category = new Category { Name = "Income", Type = TransactionType.Income, UserId = user.Id };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var transaction = new Transaction { Amount = 10, Description = "Old", Date = DateTime.UtcNow, CategoryId = category.Id };
        await context.Transactions.AddAsync(transaction);
        await context.SaveChangesAsync();

        var service = new TransactionService(context);
        var updated = await service.UpdateTransactionAsync(user.Id, transaction.Id, 99, "New");

        Assert.NotNull(updated);
        Assert.Equal(99, updated!.Amount);
        Assert.Equal("New", updated.Description);
    }
}
