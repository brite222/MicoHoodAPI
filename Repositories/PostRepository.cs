using Microsoft.EntityFrameworkCore;
using MicoHood.API.Data;
using MicoHood.API.DTOs.Common;
using MicoHood.API.DTOs.Posts;
using MicoHood.API.Entities;
using MicoHood.API.Interfaces;

namespace MicoHood.API.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> GetByIdAsync(Guid id) =>
        await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<PagedResultDto<PostResponseDto>> GetAllAsync(
        PostQueryDto query, Guid? currentUserId)
    {
        var postsQuery = _context.Posts
            .Include(p => p.User)
            .AsQueryable();

        // Filter by location if provided
        if (!string.IsNullOrWhiteSpace(query.Location))
        {
            postsQuery = postsQuery.Where(p =>
                p.Location.ToLower().Contains(query.Location.ToLower()));
        }

        var totalCount = await postsQuery.CountAsync();

        // Pagination
        var pageSize = Math.Clamp(query.PageSize, 1, 50);
        var page = Math.Max(query.Page, 1);

        var posts = await postsQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

       
        HashSet<Guid> likedPostIds = new();
        if (currentUserId.HasValue)
        {
            var postIds = posts.Select(p => p.Id).ToList();
            likedPostIds = (await _context.PostLikes
                .Where(pl => pl.UserId == currentUserId.Value
                             && postIds.Contains(pl.PostId))
                .Select(pl => pl.PostId)
                .ToListAsync()).ToHashSet();
        }

        var data = posts.Select(p => MapToDto(p, likedPostIds.Contains(p.Id))).ToList();

        return new PagedResultDto<PostResponseDto>
        {
            Data = data,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PostResponseDto?> GetByIdWithDetailsAsync(Guid id, Guid? currentUserId)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return null;

        bool liked = false;
        if (currentUserId.HasValue)
        {
            liked = await _context.PostLikes
                .AnyAsync(pl => pl.PostId == id && pl.UserId == currentUserId.Value);
        }

        return MapToDto(post, liked);
    }

    public async Task<(bool liked, int newCount)> ToggleLikeAsync(Guid postId, Guid userId)
    {
        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

        var post = await _context.Posts.FindAsync(postId)
            ?? throw new KeyNotFoundException("Post not found");

        if (existingLike is not null)
        {
           
            _context.PostLikes.Remove(existingLike);
            post.LikeCount = Math.Max(0, post.LikeCount - 1);
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (false, post.LikeCount);
        }
        else
        {
           
            var like = new PostLike { PostId = postId, UserId = userId };
            _context.PostLikes.Add(like);
            post.LikeCount += 1;
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, post.LikeCount);
        }
    }

    private static PostResponseDto MapToDto(Post post, bool liked) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        Location = post.Location,
        LikeCount = post.LikeCount,
        LikedByCurrentUser = liked,
        AuthorUsername = post.User.Username,
        UserId = post.UserId,
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt
    };
}