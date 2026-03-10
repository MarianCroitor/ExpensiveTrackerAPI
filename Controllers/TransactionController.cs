using ExpensiveTrackerAPI.Interfaces;
using ExpensiveTrackerAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var trans =  await _transactionService.GetTransactionsAsync();
        return Ok(trans);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction([FromRoute] int id)
    {
        if (id<=0)
        {
            return BadRequest();
        }
        var trans =  await _transactionService.GetTransactionAsync(id);
        if (trans == null)
        {
            return NotFound();
        }
        return Ok(trans);
    }

    [HttpPost]
    public async Task<IActionResult> PostTransactionAsync([FromBody] CreateTransactionRequest transBody)
    {
        if (transBody.CategoryId <= 0)
        {
            return BadRequest();
        }
        var created = await _transactionService.CreateTransactionAsync(
            transBody.Amount,
            transBody.Description,
            transBody.Date,
            transBody.CategoryId
        );
        if (created == null)
        {
            return NotFound();
        }
        return CreatedAtAction(nameof(GetTransaction), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTransactionAsync([FromRoute] int id, [FromBody] UpdateTransactionRequest transBody)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var updatedTrans = await _transactionService.UpdateTransactionAsync(
            id,
            transBody.Amount,
            transBody.Description
        );
        if (updatedTrans == null)
        {
            return NotFound();
        }
        return Ok(updatedTrans);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransactionAsync([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var deleted = await _transactionService.DeleteTransactionAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    
}
