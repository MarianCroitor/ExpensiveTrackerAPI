using ExpensiveTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]") ]
public class CategoryController: ControllerBase
{
    
    private readonly ICategoryService _categoryService;
    public CategoryController( ICategoryService categoryService)
    {
        
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
       var categories  = await  _categoryService.GetCategoriesAsync();
       return Ok(categories);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var category = await _categoryService.GetCategoryAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }
    
    
    
}
