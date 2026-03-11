using ExpensiveTrackerAPI.DTOs;
using ExpensiveTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/users/{userId:int}/categories")]
public class CategoryController: ControllerBase
{
    
    private readonly ICategoryService _categoryService;
    public CategoryController( ICategoryService categoryService)
    {
        
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories([FromRoute] int userId)
    {
       if (userId <= 0)
       {
           return BadRequest();
       }

       var categories  = await  _categoryService.GetCategoriesAsync(userId);
       if (categories.Count == 0)
       {
           return NotFound();
       }

       var result = categories.Select(c => new CategoryDto
       {
           Id = c.Id,
           Name = c.Name,
           Type = c.Type,
           Transactions = c.Transactions.Select(t => new TransactionDto
           {
               Id = t.Id,
               Amount = t.Amount,
               Description = t.Description,
               Date = t.Date,
               CategoryId = t.CategoryId
           }).ToList()
       }).ToList();

       return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] int userId, [FromRoute] int id)
    {
        if (userId <= 0 || id <= 0)
        {
            return BadRequest();
        }
        var category = await _categoryService.GetCategoryAsync(userId, id);
        if (category == null)
        {
            return NotFound();
        }

        var result = new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            Transactions = category.Transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                Date = t.Date,
                CategoryId = t.CategoryId
            }).ToList()
        };

        return Ok(result);
    }
    
    
    
}
