using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicoHoodApi.DTOs;
using MicoHoodApi.Helpers;
using MicoHoodApi.Interfaces;

namespace MicoHoodApi.Controllers;

[ApiController]
[Route("api/posts")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>Get all posts with optional location filter and pagination</summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <param name="location">Optional location filter e.g. Yaba, Lekki</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PostResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? location = null)
    {
        var queryParams = new PostQueryParams
        {
            Page = page,
            PageSize = pageSize,
            Location = location
        };

        var currentUserId = ClaimsHelper.TryGetUserId(User);
        var result = await _postService.GetAllPostsAsync(queryParams, currentUserId);

        return Ok(result);
    }

    /// <summary>Get a single post by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var currentUserId = ClaimsHelper.TryGetUserId(User);
        var result = await _postService.GetPostByIdAsync(id, currentUserId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>Create a new post (requires authentication)</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = ClaimsHelper.GetUserId(User);
        var result = await _postService.CreatePostAsync(dto, userId);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Toggle like/unlike on a post (requires authentication)</summary>
    [HttpPost("{id:guid}/like")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<PostResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleLike(Guid id)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var result = await _postService.ToggleLikeAsync(id, userId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}
