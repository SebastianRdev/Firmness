namespace Firmness.Application.Interfaces;

using Firmness.Domain.Entities;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
