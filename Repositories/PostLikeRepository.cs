using Microsoft.EntityFrameworkCore;
using MicoHoodApi.Data;
using MicoHoodApi.Entities;
using MicoHoodApi.Interfaces;

namespace MicoHoodApi.Repositories;

public class PostLikeRepository : IPostLikeRepository
{
    private readonly AppDbContext _context;

    public PostLikeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PostLike?> GetAsync(Guid postId, Guid userId) =>
        await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

    public async Task AddAsync(PostLike like) =>
        await _context.PostLikes.AddAsync(like);

    public Task RemoveAsync(PostLike like)
    {
        _context.PostLikes.Remove(like);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
