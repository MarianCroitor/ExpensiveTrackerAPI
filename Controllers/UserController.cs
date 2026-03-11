using ExpensiveTrackerAPI.DTOs;
using ExpensiveTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensiveTrackerAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
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
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserAsync(currentUserId.Value);
        if (user == null)
        {
            return NotFound();
        }

        var result = new List<UserDto> { MapUser(user) };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        if (!IsSameUser(id))
        {
            return Forbid();
        }

        var user = await _userService.GetUserAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(MapUser(user));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest();
        }

        var user = await _userService.CreateUserAsync(
            request.Name.Trim(),
            request.Email.Trim(),
            request.Password);

        if (user == null)
        {
            return Conflict();
        }

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, MapUser(user));
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

    private static UserDto MapUser(Models.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Categories = user.Categories.Select(c => new CategoryDto
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
            }).ToList()
        };
    }
}
