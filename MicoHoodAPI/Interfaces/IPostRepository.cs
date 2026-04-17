using MicoHood.API.DTOs.Common;
using MicoHood.API.DTOs.Posts;
using MicoHood.API.Entities;

namespace MicoHood.API.Interfaces;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task<Post?> GetByIdAsync(Guid id);
    Task<PagedResultDto<PostResponseDto>> GetAllAsync(PostQueryDto query, Guid? currentUserId);
    Task<PostResponseDto?> GetByIdWithDetailsAsync(Guid id, Guid? currentUserId);
    Task<(bool liked, int newCount)> ToggleLikeAsync(Guid postId, Guid userId);
}