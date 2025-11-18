namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Categories;
using Firmness.Application.Common;


/// <summary>
/// Manages categories in the inventory
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }      
    
    /// <summary>
    /// Retrieves all categories from the inventory
    /// </summary>
    /// <returns>A list of all categories</returns>
    /// <response code="200">Returns the list of categories</response>
    /// <response code="400">If there was an error loading the categories</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return MapResultToActionResult(result);
    }
    
    
    
    
    
    
    
    
    // ========== HELPERS (undocumented, are private) ==========

    private IActionResult MapResultToActionResult<T>(ResultOft<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);

        return MapFailure(result);
    }

    private IActionResult MapFailure<T>(ResultOft<T> result)
    {
        var error = new { error = result.ErrorMessage };
    
        // Si el mensaje indica "not found", devuelve 404
        if (result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("NotFound: {Message}", result.ErrorMessage);
            return NotFound(error);
        }
    
        // Si el mensaje indica duplicado, devuelve 409 Conflict
        if (result.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Conflict: {Message}", result.ErrorMessage);
            return Conflict(error);
        }
    
        // Por defecto 400 BadRequest
        _logger.LogWarning("BadRequest: {Message}", result.ErrorMessage);
        return BadRequest(error);
    }

    private IActionResult MapFailure(Result result)
    {
        var message = result.ErrorMessage ?? "An error occurred";
        if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = message });

        return BadRequest(new { error = message });
    }
}