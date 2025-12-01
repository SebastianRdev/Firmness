namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return MapResultToActionResult(result);
    }
    /// <summary>
    /// Retrieves a specific category by its ID
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>The category details</returns>
    /// <response code="200">Returns the category</response>
    /// <response code="404">If the category is not found</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Creates a new category in the inventory
    /// </summary>
    /// <param name="createDto">The category data</param>
    /// <returns>The newly created category</returns>
    /// <response code="201">Returns the newly created category</response>
    /// <response code="400">If the data is invalid or the category code already exists</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/categories
    ///     {
    ///        "name": "Portland Cement",
    ///        "category": "Construction Materials",
    ///        "description": "High quality cement for construction",
    ///        "code": "CEM-001",
    ///        "price": 25000,
    ///        "stock": 100
    ///     }
    ///
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _categoryService.CreateAsync(createDto);

        if (!result.IsSuccess)
            return MapFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }
    
    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <param name="updateDto">The updated category data</param>
    /// <returns>The updated category</returns>
    /// <response code="200">Returns the updated category</response>
    /// <response code="400">If the data is invalid or IDs don't match</response>
    /// <response code="404">If the category is not found</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest(new { error = "ID mismatch between route and payload" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _categoryService.UpdateAsync(updateDto);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Deletes a category from the inventory
    /// </summary>
    /// <param name="id">The category ID to delete</param>
    /// <response code="204">If the category was successfully deleted</response>
    /// <response code="404">If the category is not found</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (!result.IsSuccess)
            return MapFailure(result);

        return NoContent();
    }

    /// <summary>
    /// Searches categories by name or code
    /// </summary>
    /// <param name="term">The search term (minimum 2 characters)</param>
    /// <returns>A list of matching categories</returns>
    /// <response code="200">Returns the list of matching categories</response>
    /// <response code="400">If the search term is invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _categoryService.SearchAsync(term);
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