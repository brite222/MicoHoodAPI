using Microsoft.EntityFrameworkCore;
using MicoHoodApi.Data;
using MicoHoodApi.DTOs;
using MicoHoodApi.Entities;
using MicoHoodApi.Interfaces;

namespace MicoHoodApi.Repositories;

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _context;

    public PostRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Post?> GetByIdAsync(Guid id) =>
        await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<PagedResult<Post>> GetAllAsync(PostQueryParams queryParams)
    {
        var query = _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .AsQueryable();

        // Optional location filter (case-insensitive)
        if (!string.IsNullOrWhiteSpace(queryParams.Location))
        {
            var loc = queryParams.Location.ToLower();
            query = query.Where(p => p.Location.ToLower().Contains(loc));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return new PagedResult<Post>
        {
            Items = items,
            TotalCount = totalCount,
            Page = queryParams.Page,
            PageSize = queryParams.PageSize
        };
    }

    public async Task AddAsync(Post post) =>
        await _context.Posts.AddAsync(post);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
