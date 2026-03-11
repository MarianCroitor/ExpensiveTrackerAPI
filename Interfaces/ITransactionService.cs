namespace ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.Models;

public interface ITransactionService
{
    Task<List<Transaction>> GetTransactionsAsync(int userId);
    Task<List<Transaction>?> GetCategoryTransactionsAsync(int userId, int categoryId);
    Task<Transaction?> GetTransactionAsync(int userId, int id);
    Task<Transaction?> CreateTransactionAsync(int userId, int categoryId, decimal amount, string description, DateTime date);
    Task<Transaction?> UpdateTransactionAsync(int userId, int id, decimal amount, string description);
    Task<bool> DeleteTransactionAsync(int userId, int id);
    
}
