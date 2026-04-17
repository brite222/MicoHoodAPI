using MicoHoodApi.DTOs;

namespace MicoHoodApi.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
}

public interface IPostService
{
    Task<ApiResponse<PostResponseDto>> CreatePostAsync(CreatePostDto dto, Guid userId);
    Task<ApiResponse<PostResponseDto>> GetPostByIdAsync(Guid id, Guid? currentUserId);
    Task<ApiResponse<PagedResult<PostResponseDto>>> GetAllPostsAsync(PostQueryParams queryParams, Guid? currentUserId);
    Task<ApiResponse<PostResponseDto>> ToggleLikeAsync(Guid postId, Guid userId);
}
