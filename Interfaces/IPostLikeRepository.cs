using MicoHoodApi.Entities;

namespace MicoHoodApi.Interfaces;

public interface IPostLikeRepository
{
    Task<PostLike?> GetAsync(Guid postId, Guid userId);
    Task AddAsync(PostLike like);
    Task RemoveAsync(PostLike like);
    Task SaveChangesAsync();
}
