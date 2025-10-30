namespace Firmness.Infrastructure.Services.Identity;
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> LogoutAsync();
    //Task<AuthResult> RegisterAsync(RegisterViewModel model);
}
