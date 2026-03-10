namespace ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.Models;

public interface ITransactionService
{
    Task<List<Transaction>> GetTransactionsAsync();
    Task<Transaction?> GetTransactionAsync(int id);
    Task<Transaction?> CreateTransactionAsync(decimal amount, string description, DateTime date, int categoryId);
    Task<Transaction?> UpdateTransactionAsync(int id, decimal amount, string description);
    Task<bool> DeleteTransactionAsync(int id);
    
}
