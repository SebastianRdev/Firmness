namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Products;
using Firmness.Application.Common;

/// <summary>
/// Manages products in the inventory
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseApiController
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }      
    
    /// <summary>
    /// Retrieves all products from the inventory
    /// </summary>
    /// <returns>A list of all products</returns>
    /// <response code="200">Returns the list of products</response>
    /// <response code="400">If there was an error loading the products</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productService.GetAllAsync();
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Retrieves a specific product by its ID
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <returns>The product details</returns>
    /// <response code="200">Returns the product</response>
    /// <response code="404">If the product is not found</response>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetByIdAsync(id);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Creates a new product in the inventory
    /// </summary>
    /// <param name="createDto">The product data</param>
    /// <returns>The newly created product</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the data is invalid or the product code already exists</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/products
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
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.CreateAsync(createDto);

        if (!result.IsSuccess)
            return MapFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">The product ID</param>
    /// <param name="updateDto">The updated product data</param>
    /// <returns>The updated product</returns>
    /// <response code="200">Returns the updated product</response>
    /// <response code="400">If the data is invalid or IDs don't match</response>
    /// <response code="404">If the product is not found</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest(new { error = "ID mismatch between route and payload" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _productService.UpdateAsync(updateDto);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Deletes a product from the inventory
    /// </summary>
    /// <param name="id">The product ID to delete</param>
    /// <response code="204">If the product was successfully deleted</response>
    /// <response code="404">If the product is not found</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.IsSuccess)
            return MapFailure(result);

        return NoContent();
    }

    /// <summary>
    /// Searches products by name or code
    /// </summary>
    /// <param name="term">The search term (minimum 2 characters)</param>
    /// <returns>A list of matching products</returns>
    /// <response code="200">Returns the list of matching products</response>
    /// <response code="400">If the search term is invalid</response>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _productService.SearchAsync(term);
        return MapResultToActionResult(result);
    }

}