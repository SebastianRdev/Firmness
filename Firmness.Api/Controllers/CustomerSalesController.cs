namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Sales;
using Firmness.Application.Common;

/// <summary>
/// Manages customer sales with PDF receipt generation and email sending
/// </summary>
[ApiController]
[Route("api/customer/sales")]
[Authorize] // Any authenticated user can create sales
public class CustomerSalesController : ControllerBase
{
    private readonly ICustomerSaleService _saleService;
    private readonly ILogger<CustomerSalesController> _logger;

    public CustomerSalesController(ICustomerSaleService saleService, ILogger<CustomerSalesController> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new sale with PDF receipt and email notification
    /// </summary>
    /// <param name="createDto">Sale data including customer and products</param>
    /// <returns>The created sale with receipt information</returns>
    /// <response code="201">Returns the newly created sale</response>
    /// <response code="400">If the data is invalid or inventory is insufficient</response>
    [HttpPost]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _saleService.CreateSaleWithReceiptAsync(createDto);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return CreatedAtAction(nameof(GetSaleById), new { id = result.Data.Id }, result.Data);
    }

    /// <summary>
    /// Retrieves a sale by its ID
    /// </summary>
    /// <param name="id">The sale ID</param>
    /// <returns>The sale details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SaleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleById(int id)
    {
        var result = await _saleService.GetSaleByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }
}
