using MicoHoodApi.DTOs;
using MicoHoodApi.Entities;
using MicoHoodApi.Interfaces;

namespace MicoHoodApi.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepo;
    private readonly IPostLikeRepository _likeRepo;
    private readonly IUserRepository _userRepo;

    public PostService(
        IPostRepository postRepo,
        IPostLikeRepository likeRepo,
        IUserRepository userRepo)
    {
        _postRepo = postRepo;
        _likeRepo = likeRepo;
        _userRepo = userRepo;
    }

    public async Task<ApiResponse<PostResponseDto>> CreatePostAsync(CreatePostDto dto, Guid userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<PostResponseDto>.Fail("User not found.");

        var post = new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            Location = dto.Location,
            UserId = userId
        };

        await _postRepo.AddAsync(post);
        await _postRepo.SaveChangesAsync();

        // Reload so User navigation is populated
        var created = await _postRepo.GetByIdAsync(post.Id);

        return ApiResponse<PostResponseDto>.Ok(MapToDto(created!, userId), "Post created successfully.");
    }

    public async Task<ApiResponse<PostResponseDto>> GetPostByIdAsync(Guid id, Guid? currentUserId)
    {
        var post = await _postRepo.GetByIdAsync(id);
        if (post == null)
            return ApiResponse<PostResponseDto>.Fail("Post not found.");

        return ApiResponse<PostResponseDto>.Ok(MapToDto(post, currentUserId));
    }

    public async Task<ApiResponse<PagedResult<PostResponseDto>>> GetAllPostsAsync(
        PostQueryParams queryParams, Guid? currentUserId)
    {
        var paged = await _postRepo.GetAllAsync(queryParams);

        var result = new PagedResult<PostResponseDto>
        {
            Items = paged.Items.Select(p => MapToDto(p, currentUserId)).ToList(),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };

        return ApiResponse<PagedResult<PostResponseDto>>.Ok(result);
    }

    public async Task<ApiResponse<PostResponseDto>> ToggleLikeAsync(Guid postId, Guid userId)
    {
        var post = await _postRepo.GetByIdAsync(postId);
        if (post == null)
            return ApiResponse<PostResponseDto>.Fail("Post not found.");

        var existingLike = await _likeRepo.GetAsync(postId, userId);

        if (existingLike != null)
        {
            // Unlike
            await _likeRepo.RemoveAsync(existingLike);
            post.LikesCount = Math.Max(0, post.LikesCount - 1);
            await _postRepo.SaveChangesAsync();
        }
        else
        {
            // Like
            var like = new PostLike { PostId = postId, UserId = userId };
            await _likeRepo.AddAsync(like);
            post.LikesCount++;
            await _postRepo.SaveChangesAsync();
        }

        // Reload updated post
        var updated = await _postRepo.GetByIdAsync(postId);
        var message = existingLike != null ? "Post unliked." : "Post liked.";

        return ApiResponse<PostResponseDto>.Ok(MapToDto(updated!, userId), message);
    }

    private static PostResponseDto MapToDto(Post post, Guid? currentUserId) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Content = post.Content,
        Location = post.Location,
        LikesCount = post.LikesCount,
        LikedByCurrentUser = currentUserId.HasValue && post.Likes.Any(l => l.UserId == currentUserId.Value),
        AuthorUsername = post.User?.Username ?? "Unknown",
        CreatedAt = post.CreatedAt,
        UpdatedAt = post.UpdatedAt
    };
}
