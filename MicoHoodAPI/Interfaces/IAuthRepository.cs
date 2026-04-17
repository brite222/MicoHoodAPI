using MicoHood.API.Entities;

namespace MicoHood.API.Interfaces;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<User> CreateUserAsync(User user);
    Task<User?> GetByEmailAsync(string email);
}