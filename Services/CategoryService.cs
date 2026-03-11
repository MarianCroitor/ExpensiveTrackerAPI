using ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpensiveTrackerAPI.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;
    
    public CategoryService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Category>> GetCategoriesAsync(int userId)
    {
        return await _context.Categories
            .Include(c => c.Transactions)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(int userId, int categoryId)
    {
        return await _context.Categories
            .Include(c => c.Transactions)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId);
    }
}
