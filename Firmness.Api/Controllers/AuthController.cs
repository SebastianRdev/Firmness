namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;
using Firmness.Application.DTOs.Auth;
using Firmness.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid credentials" });

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponseDto
        {
            Id = user.Id,
            Token = token,
            Email = user.Email!,
            FullName = user.FullName ?? user.UserName!,
            Roles = roles
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
            return BadRequest(new { message = "User already exists" });

        var user = new Customer
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = $"{model.FirstName} {model.LastName}",
            Address = model.Address,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Assign default role "Customer"
        await _userManager.AddToRoleAsync(user, "Customer");

        var roles = new List<string> { "Customer" };
        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponseDto
        {
            Id = user.Id,
            Token = token,
            Email = user.Email!,
            FullName = user.FullName!,
            Roles = roles
        });
    }
}
