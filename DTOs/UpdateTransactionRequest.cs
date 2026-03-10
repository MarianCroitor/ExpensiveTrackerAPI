namespace ExpensiveTrackerAPI.DTOs;

public class UpdateTransactionRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
