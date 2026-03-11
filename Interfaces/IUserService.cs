using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.Interfaces;

public interface IUserService
{
    Task<List<User>> GetUsersAsync();
    Task<User?> GetUserAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> ValidateUserAsync(string email, string password);
    Task<User?> CreateUserAsync(string name, string email, string password);
}
