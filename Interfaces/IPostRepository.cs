using MicoHoodApi.DTOs;
using MicoHoodApi.Entities;

namespace MicoHoodApi.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    Task<PagedResult<Post>> GetAllAsync(PostQueryParams queryParams);
    Task AddAsync(Post post);
    Task SaveChangesAsync();
}
