using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonixy.UserService.Application.DTOs;
using Sonixy.UserService.Application.Services;

namespace Sonixy.UserService.Api.Controllers;

/// <summary>
/// User profile management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
{
    private readonly ILogger<UsersController> _logger = logger;

    [HttpPost("presigned-url")]
    [ProducesResponseType(typeof(PresignedUrlResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GeneratePresignedUrl([FromBody] PresignedUrlRequestDto request)
    {
        var (uploadUrl, objectKey, publicUrl) = await userService.GeneratePresignedUrlAsync(request.FileName, request.ContentType);
        return Ok(new PresignedUrlResponseDto(uploadUrl, objectKey, publicUrl));
    }

    /// <summary>
    /// Creates a new user profile
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/users
    ///     {
    ///       "firstName": "Nhat",
    ///       "lastName": "Cuong",
    ///       "email": "nhat.cuong@example.com",
    ///       "bio": "Software Engineer at Fujinet",
    ///       "avatarUrl": "https://example.com/avatar.jpg"
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">User creation data</param>
    /// <returns>The newly created user</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Invalid request - validation failed or email already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        try
        {
            var user = await userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a user profile by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User profile</returns>
    /// <response code="200">User found</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await userService.GetUserByIdAsync(id);
        
        if (user is null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }

    /// <summary>
    /// Updates a user profile
    /// </summary>
    /// <remarks>
    /// Only provided fields will be updated. Email cannot be changed.
    /// 
    /// Sample request:
    /// 
    ///     PATCH /api/users/507f1f77bcf86cd799439011
    ///     {
    ///       "firstName": "Nhat",
    ///       "lastName": "Cuong",
    ///       "bio": "Junior Developer"
    ///     }
    /// 
    /// </remarks>
    /// <param name="id">User ID</param>
    /// <param name="dto">Update data</param>
    /// <returns>Updated user profile</returns>
    /// <response code="200">User updated successfully</response>
    /// <response code="404">User not found</response>
    /// <response code="400">Invalid user ID</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var user = await userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Batch retrieves multiple users by their IDs
    /// </summary>
    /// <remarks>
    /// Efficiently fetches multiple user profiles in a single request.
    /// Used internally by other services to enrich data.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/users/batch
    ///     {
    ///       "ids": ["507f1f77bcf86cd799439011", "507f191e810c19729de860ea"]
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">List of user IDs</param>
    /// <returns>List of user profiles</returns>
    /// <response code="200">Users retrieved successfully</response>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersBatch([FromBody] BatchUserRequest request)
    {
        var users = await userService.GetUsersBatchAsync(request.Ids);
        return Ok(users);
    }
    /// <summary>
    /// Retrieves the current authenticated user's profile
    /// </summary>
    /// <returns>Current user profile</returns>
    /// <response code="200">User found</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">User not found</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await userService.GetUserByIdAsync(userId);
        
        if (user is null)
            return NotFound(new { error = "User not found" });

        return Ok(user);
    }
}

public record BatchUserRequest(IEnumerable<string> Ids);
