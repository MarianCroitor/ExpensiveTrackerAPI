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
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }
}