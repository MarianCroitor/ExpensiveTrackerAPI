using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync(int userId);
    Task<Category?> GetCategoryAsync(int userId, int categoryId);
}
