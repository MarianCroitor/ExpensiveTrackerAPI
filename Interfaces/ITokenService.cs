using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
