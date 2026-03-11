using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensiveTrackerAPI.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Categories)
            .ThenInclude(c => c.Transactions)
            .ToListAsync();
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Categories)
            .ThenInclude(c => c.Transactions)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success ? user : null;
    }

    public async Task<User?> CreateUserAsync(string name, string email, string password)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == email);
        if (exists)
        {
            return null;
        }

        var user = new User
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, password);

        user.Categories = new List<Category>
        {
            new() { Name = "Income", Type = TransactionType.Income, User = user },
            new() { Name = "Expense", Type = TransactionType.Expense, User = user },
            new() { Name = "Investment", Type = TransactionType.Investment, User = user }
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
