using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpensiveTrackerAPI.Tests;

public class UserServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailUnique_CreatesUser()
    {
        await using var context = CreateContext();
        var service = new UserService(context);

        var user = await service.CreateUserAsync("Mira", "mira@example.com", "pass1234");

        Assert.NotNull(user);
        Assert.Equal("mira@example.com", user!.Email);
        Assert.Equal(3, user.Categories.Count);
        Assert.False(string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailExists_ReturnsNull()
    {
        await using var context = CreateContext();
        var service = new UserService(context);

        var first = await service.CreateUserAsync("Mira", "mira@example.com", "pass1234");
        var second = await service.CreateUserAsync("Other", "mira@example.com", "pass9999");

        Assert.NotNull(first);
        Assert.Null(second);
    }

    [Fact]
    public async Task ValidateUserAsync_WithValidPassword_ReturnsUser()
    {
        await using var context = CreateContext();
        var service = new UserService(context);

        await service.CreateUserAsync("Mira", "mira@example.com", "pass1234");
        var user = await service.ValidateUserAsync("mira@example.com", "pass1234");

        Assert.NotNull(user);
    }

    [Fact]
    public async Task ValidateUserAsync_WithWrongPassword_ReturnsNull()
    {
        await using var context = CreateContext();
        var service = new UserService(context);

        await service.CreateUserAsync("Mira", "mira@example.com", "pass1234");
        var user = await service.ValidateUserAsync("mira@example.com", "badpass");

        Assert.Null(user);
    }
}
