using ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/users/{userId:int}")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions([FromRoute] int userId)
    {
        if (userId <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var trans =  await _transactionService.GetTransactionsAsync(userId);
        var result = trans.Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description,
            Date = t.Date,
            CategoryId = t.CategoryId
        }).ToList();

        return Ok(result);
    }

    [HttpGet("categories/{categoryId:int}/transactions")]
    public async Task<IActionResult> GetCategoryTransactions([FromRoute] int userId, [FromRoute] int categoryId)
    {
        if (userId <= 0 || categoryId <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var trans = await _transactionService.GetCategoryTransactionsAsync(userId, categoryId);
        if (trans == null)
        {
            return NotFound();
        }

        var result = trans.Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description,
            Date = t.Date,
            CategoryId = t.CategoryId
        }).ToList();

        return Ok(result);
    }

    [HttpGet("transactions/{id:int}")]
    public async Task<IActionResult> GetTransaction([FromRoute] int userId, [FromRoute] int id)
    {
        if (userId <= 0 || id <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var trans = await _transactionService.GetTransactionAsync(userId, id);
        if (trans == null)
        {
            return NotFound();
        }

        var result = new TransactionDto
        {
            Id = trans.Id,
            Amount = trans.Amount,
            Description = trans.Description,
            Date = trans.Date,
            CategoryId = trans.CategoryId
        };

        return Ok(result);
    }

    [HttpPost("categories/{categoryId:int}/transactions")]
    public async Task<IActionResult> PostTransactionAsync(
        [FromRoute] int userId,
        [FromRoute] int categoryId,
        [FromBody] CreateTransactionRequest transBody)
    {
        if (userId <= 0 || categoryId <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var created = await _transactionService.CreateTransactionAsync(
            userId,
            categoryId,
            transBody.Amount,
            transBody.Description,
            transBody.Date
        );
        if (created == null)
        {
            return NotFound();
        }

        var result = new TransactionDto
        {
            Id = created.Id,
            Amount = created.Amount,
            Description = created.Description,
            Date = created.Date,
            CategoryId = created.CategoryId
        };

        return CreatedAtAction(nameof(GetTransaction), new { userId = userId, id = created.Id }, result);
    }

    [HttpPut("transactions/{id:int}")]
    public async Task<IActionResult> PutTransactionAsync(
        [FromRoute] int userId,
        [FromRoute] int id,
        [FromBody] UpdateTransactionRequest transBody)
    {
        if (userId <= 0 || id <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var updatedTrans = await _transactionService.UpdateTransactionAsync(
            userId,
            id,
            transBody.Amount,
            transBody.Description
        );
        if (updatedTrans == null)
        {
            return NotFound();
        }

        var result = new TransactionDto
        {
            Id = updatedTrans.Id,
            Amount = updatedTrans.Amount,
            Description = updatedTrans.Description,
            Date = updatedTrans.Date,
            CategoryId = updatedTrans.CategoryId
        };

        return Ok(result);
    }

    [HttpDelete("transactions/{id:int}")]
    public async Task<IActionResult> DeleteTransactionAsync([FromRoute] int userId, [FromRoute] int id)
    {
        if (userId <= 0 || id <= 0)
        {
            return BadRequest();
        }
        if (!IsSameUser(userId))
        {
            return Forbid();
        }

        var deleted = await _transactionService.DeleteTransactionAsync(userId, id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }

    private bool IsSameUser(int userId)
    {
        var currentUserId = GetCurrentUserId();
        return currentUserId != null && currentUserId.Value == userId;
    }

}
