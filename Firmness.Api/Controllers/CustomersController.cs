namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;

/// <summary>
/// Manages customer CRUD operations
/// </summary>
[ApiController]
[Route("api/customers")]
// [Authorize(Roles = "Admin")]
public class CustomersController : BaseApiController
{
    private readonly ICustomerCrudService _customerCrudService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerCrudService customerCrudService,
        ILogger<CustomersController> logger)
    {
        _customerCrudService = customerCrudService;
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
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var result = await _customerCrudService.GetAllAsync(page, pageSize);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Retrieves a specific customer by its ID
    /// </summary>
    /// <param name="id">The customer ID</param>
    /// <returns>The customer details</returns>
    /// <response code="200">Returns the customer</response>
    /// <response code="404">If the customer was not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _customerCrudService.GetByIdAsync(id);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _customerCrudService.CreateAsync(createDto);

        if (!result.IsSuccess)
            return MapFailure(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Updates an existing customer
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _customerCrudService.UpdateAsync(updateDto);
        return MapResultToActionResult(result);
    }

    /// <summary>
    /// Deletes a customer
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _customerCrudService.DeleteAsync(id);

        if (!result.IsSuccess)
            return MapFailure(result);

        return NoContent();
    }

    /// <summary>
    /// Searches customers by term
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        var result = await _customerCrudService.SearchAsync(term);
        return MapResultToActionResult(result);
    }
}