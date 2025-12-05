namespace Firmness.Application.Interfaces;

using Firmness.Domain.Entities;

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user and roles.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <param name="roles">The list of roles assigned to the user.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateToken(ApplicationUser user, IList<string> roles);
}
