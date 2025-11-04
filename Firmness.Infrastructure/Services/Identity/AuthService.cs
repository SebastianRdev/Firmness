namespace Firmness.Infrastructure.Services.Identity;

using Microsoft.AspNetCore.Identity;
using Domain.Entities;

public class AuthService : IAuthService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return AuthResult.Failure("Usuario no encontrado.");

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
        if (!result.Succeeded)
            return AuthResult.Failure("Credenciales inválidas.");

        return AuthResult.SuccessResult();
    }

    public async Task<AuthResult> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return AuthResult.SuccessResult();
    }
}
