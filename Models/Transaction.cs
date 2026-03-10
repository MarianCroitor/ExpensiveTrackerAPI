namespace ExpensiveTrackerAPI.Models;
using System.Text.Json.Serialization;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    [JsonIgnore]
    public Category? Category { get; set; }
}
