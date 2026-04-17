using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicoHood.API.DTOs.Posts;
using MicoHood.API.Entities;
using MicoHood.API.Interfaces;

namespace MicoHood.API.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepo;

    public PostsController(IPostRepository postRepo)
    {
        _postRepo = postRepo;
    }

    private Guid? GetCurrentUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PostQueryDto query)
    {
        var userId = GetCurrentUserId();
        var result = await _postRepo.GetAllAsync(query, userId);
        return Ok(result);
    }

   
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();
        var post = await _postRepo.GetByIdWithDetailsAsync(id, userId);

        if (post is null)
            return NotFound(new { message = "Post not found" });

        return Ok(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var post = new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            Location = dto.Location,
            UserId = userId.Value
        };

        var created = await _postRepo.CreateAsync(post);
        var response = await _postRepo.GetByIdWithDetailsAsync(created.Id, userId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

   
    [Authorize]
    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> ToggleLike(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

       
        var post = await _postRepo.GetByIdAsync(id);
        if (post is null)
            return NotFound(new { message = "Post not found" });

        var (liked, newCount) = await _postRepo.ToggleLikeAsync(id, userId.Value);

        return Ok(new
        {
            liked,
            likeCount = newCount,
            message = liked ? "Post liked" : "Post unliked"
        });
    }
}