namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Firmness.Domain.Entities;
using Firmness.Application.DTOs.Auth;
using Firmness.Application.Interfaces;

/// <summary>
/// Controller responsible for user authentication and registration.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="jwtService">The JWT service.</param>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Authenticates a user and generates a JWT token.
    /// </summary>
    /// <param name="model">The login credentials.</param>
    /// <returns>An authentication response containing the token and user details.</returns>
    /// <response code="200">Returns the authentication response.</response>
    /// <response code="400">If the request model is invalid.</response>
    /// <response code="401">If the credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Registers a new customer user.
    /// </summary>
    /// <param name="model">The registration details.</param>
    /// <returns>An authentication response containing the token and user details.</returns>
    /// <response code="200">Returns the authentication response.</response>
    /// <response code="400">If the request model is invalid or the user already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
