namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;

/// <summary>
/// Manages customers in the inventory
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }      
    
    /// <summary>
    /// Retrieves all customers from the inventory
    /// </summary>
    /// <returns>A list of all customers</returns>
    /// <response code="200">Returns the list of customers</response>
    /// <response code="400">If there was an error loading the customers</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _customerService.GetAllAsync();
        return MapResultToActionResult(result);
    }
    /// <summary>
    /// Retrieves a specific customer by its ID
    /// </summary>
    /// <param name="id">The customer ID</param>
    /// <returns>The customer details</returns>
    /// <response code="200">Returns the customer</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return MapResultToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _customerService.CreateAsync(createDto);

        if (!result.IsSuccess)
            return MapFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }
    
    /// <summary>
    /// Updates an existing customer
    /// </summary>
    /// <param name="id">The customer ID</param>
    /// <param name="updateDto">The updated customer data</param>
    /// <returns>The updated customer</returns>
    /// <response code="200">Returns the updated customer</response>
    /// <response code="400">If the data is invalid or IDs don't match</response>
    /// <response code="404">If the customer is not found</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto updateDto)
    {
        if (id != updateDto.Id)
            return BadRequest(new { error = "ID mismatch between route and payload" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _customerService.UpdateAsync(updateDto);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Deletes a customer from the inventory
    /// </summary>
    /// <param name="id">The customer ID to delete</param>
    /// <response code="204">If the customer was successfully deleted</response>
    /// <response code="404">If the customer is not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);

        if (!result.IsSuccess)
            return MapFailure(result);

        return NoContent();
    }

    /// <summary>
    /// Searches customers by name or code
    /// </summary>
    /// <param name="term">The search term (minimum 2 characters)</param>
    /// <returns>A list of matching customers</returns>
    /// <response code="200">Returns the list of matching customers</response>
    /// <response code="400">If the search term is invalid</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _customerService.SearchAsync(term);
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