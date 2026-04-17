using MicoHood.API.Entities;

namespace MicoHood.API.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpiry();
}