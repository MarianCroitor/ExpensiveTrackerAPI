using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int id);
    Task<User> CreateUserAsync(string name, string email);
}
