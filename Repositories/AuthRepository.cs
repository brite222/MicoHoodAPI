using Microsoft.EntityFrameworkCore;
using MicoHood.API.Data;
using MicoHood.API.Entities;
using MicoHood.API.Interfaces;

namespace MicoHood.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email.ToLower());

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _context.Users.AnyAsync(u => u.Username == username.ToLower());

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
}