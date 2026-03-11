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

    public async Task<User> CreateUserAsync(string name, string email)
    {
        var user = new User
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
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
