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
    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .ToListAsync();
    }

    public async Task<Transaction?> GetTransactionAsync(int id)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Transaction?> CreateTransactionAsync(decimal amount, string description, DateTime date, int categoryId)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
        if (!categoryExists)
        {
            return null;
        }
        var trans = new Transaction
        {
            Amount = amount,
            Description = description,
            Date = date,
            CategoryId = categoryId

        };
        await _context.Transactions.AddAsync(trans);
        await _context.SaveChangesAsync();
        return trans;
    }
    
    public async Task<Transaction?> UpdateTransactionAsync(int id, decimal amount, string description)
    {
        var trans = await _context.Transactions.FindAsync(id);
        if (trans == null)
        {
            return null;
        }
        trans.Amount = amount;
        trans.Description = description;
        await _context.SaveChangesAsync();
        return trans;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var trans =  await _context.Transactions.FindAsync(id);
        if (trans == null)
        {
            return false;
        }
        _context.Transactions.Remove(trans);
        await _context.SaveChangesAsync();
        return true;
    }
    
}
