using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> GetCategoryAsync(int id);
}