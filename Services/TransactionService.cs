namespace ExpensiveTrackerAPI.Services;
using ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.Data;
using ExpensiveTrackerAPI.Models;
using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    public TransactionService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Transaction>> GetTransactionsAsync(int userId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Category != null && t.Category.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Transaction>?> GetCategoryTransactionsAsync(int userId, int categoryId)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == categoryId && c.UserId == userId);
        if (!categoryExists)
        {
            return null;
        }

        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionAsync(int userId, int id)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.Category != null && t.Category.UserId == userId);
    }

    public async Task<Transaction?> CreateTransactionAsync(int userId, int categoryId, decimal amount, string description, DateTime date)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);
        if (category == null)
        {
            return null;
        }
        var trans = new Transaction
        {
            Amount = amount,
            Description = description,
            Date = date,
            CategoryId = category.Id

        };
        await _context.Transactions.AddAsync(trans);
        await _context.SaveChangesAsync();
        return trans;
    }
    
    public async Task<Transaction?> UpdateTransactionAsync(int userId, int id, decimal amount, string description)
    {
        var trans = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.Category != null && t.Category.UserId == userId);
        if (trans == null)
        {
            return null;
        }
        trans.Amount = amount;
        trans.Description = description;
        await _context.SaveChangesAsync();
        return trans;
    }

    public async Task<bool> DeleteTransactionAsync(int userId, int id)
    {
        var trans = await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id && t.Category != null && t.Category.UserId == userId);
        if (trans == null)
        {
            return false;
        }
        _context.Transactions.Remove(trans);
        await _context.SaveChangesAsync();
        return true;
    }
    
}
