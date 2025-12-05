using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Services.Identity;
using Firmness.WebAdmin.Models;

namespace Firmness.WebAdmin.Controllers;

/// <summary>
/// Controller responsible for managing user account actions such as login and logout.
/// </summary>
public class AccountController : Controller
{
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Displays the login view.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login() => View();

    /// <summary>
    /// Handles the login process.
    /// </summary>
    /// <param name="model">The login view model containing user credentials.</param>
    /// <returns>Redirects to the dashboard on success, or returns the view with errors on failure.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken] // Protección contra ataques CSRF
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model.Email, model.Password);
    
        if (!result.Success)
            ModelState.AddModelError("", result.Message);

        return result.Success ? RedirectToAction("Index", "Dashboard") : View(model);
    }

    /// <summary>
    /// Handles the logout process.
    /// </summary>
    /// <returns>Redirects to the home page.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}