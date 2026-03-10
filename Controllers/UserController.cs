using ExpensiveTrackerAPI.DTOs;
using ExpensiveTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        var result = users.Select(u => new UserDto
        {
            Name = u.Name,
            Email = u.Email,
            Categories = u.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var user = await _userService.GetUserAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = new UserDto
        {
            Name = user.Name,
            Email = user.Email,
            Categories = user.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            }).ToList()
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest();
        }

        var user = await _userService.CreateUserAsync(request.Name.Trim(), request.Email.Trim());
        var result = new UserDto
        {
            Name = user.Name,
            Email = user.Email,
            Categories = user.Categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type
            }).ToList()
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, result);
    }
}
