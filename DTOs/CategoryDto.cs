using ExpensiveTrackerAPI.Models;

namespace ExpensiveTrackerAPI.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}
