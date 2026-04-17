using MicoHoodApi.Entities;

namespace MicoHoodApi.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
