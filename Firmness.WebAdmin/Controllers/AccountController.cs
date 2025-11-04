using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Firmness.Domain.Entities;
using Firmness.Infrastructure.Services.Identity;
using Firmness.WebAdmin.Models;

namespace Firmness.WebAdmin.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}