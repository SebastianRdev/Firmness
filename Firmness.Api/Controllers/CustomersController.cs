namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;
using OfficeOpenXml;

/// <summary>
/// Manages customers in the inventory
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
public class CustomersController : ControllerBase
{
    private readonly IExcelService _excelService;
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerService customerService,
        IExcelService excelService,
        ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _excelService = excelService;
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
        var result = await _customerService.GetAllAsync(page, pageSize);
        return MapResultToActionResult(result);
    }
    /// <summary>
    /// Retrieves a specific customer by its ID
    /// </summary>
    /// <param name="id">The customer ID</param>
    /// <returns>The customer details</returns>
    /// <response code="200">Returns the customer</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
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
    [HttpPut("{id:Guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto updateDto)
    {
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
    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
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
    
    // Obtener roles del usuario
    [HttpGet("{id}/roles")]
    public async Task<IActionResult> GetUserRoles(Guid id)
    {
        try
        {
            var roles = await _customerService.GetUserRolesAsync(id);
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Obtener lista de roles disponibles
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _customerService.GetAllRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Actualizar rol del usuario
    [HttpPut("{id}/roles")]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] string role)
    {
        try
        {
            await _customerService.UpdateUserRoleAsync(id, role);
            return Ok(new { message = "Role updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Step 1: Extract headers from Excel and return them for user confirmation
    /// </summary>
    [HttpPost("import/headers")]
    [ProducesResponseType(typeof(ResultOft<ExcelHeadersResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExtractHeaders(IFormFile file, [FromForm] string entityType)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "File is empty" });

        if (string.IsNullOrWhiteSpace(entityType))
            return BadRequest(new { error = "Entity type is required" });

        _logger.LogInformation("Extracting headers from file: {FileName}, EntityType: {EntityType}", 
            file.FileName, entityType);

        var result = await _excelService.ExtractHeadersFromExcelAsync(file, entityType);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to extract headers: {Error}", result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }

        _logger.LogInformation("Successfully extracted {Count} headers", 
            result.Data?.OriginalHeaders?.Count ?? 0);

        // ✅ Devolver el ResultOft completo, NO solo result.Data
        return Ok(result);
    }

    /// <summary>
    /// Step 2: Use AI to correct column names if needed
    /// </summary>
    [HttpPost("import/correct-headers")]
    [ProducesResponseType(typeof(ResultOft<ExcelHeadersResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CorrectHeaders([FromBody] CorrectHeadersRequest request)
    {
        if (request.OriginalHeaders == null || !request.OriginalHeaders.Any())
            return BadRequest(new { error = "Original headers are required" });

        if (request.CorrectHeaders == null || !request.CorrectHeaders.Any())
            return BadRequest(new { error = "Correct headers template is required" });

        _logger.LogInformation("Correcting {Count} headers with AI", request.OriginalHeaders.Count);

        var result = await _excelService.CorrectColumnNamesAsync(
            request.OriginalHeaders, 
            request.CorrectHeaders);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("AI correction failed: {Error}", result.ErrorMessage);
            return BadRequest(new { error = result.ErrorMessage });
        }

        _logger.LogInformation("AI correction successful. WasCorrected: {WasCorrected}", 
            result.Data?.WasCorrected ?? false);

        // ✅ Devolver el ResultOft completo
        return Ok(result);
    }

    /// <summary>
    /// Step 3: Import the Excel data with corrected or confirmed headers
    /// </summary>
    [HttpPost("import-excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportExcel(IFormFile file, [FromForm] string entityType)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file selected" });

        if (string.IsNullOrWhiteSpace(entityType))
            return BadRequest(new { error = "Entity type is required" });

        // Aquí el CustomerService ya debería manejar la importación completa
        var result = await _customerService.ImportFromExcelAsync(file, entityType);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { message = "Data imported successfully" });
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
    public class CorrectHeadersRequest
    {
        public List<string> OriginalHeaders { get; set; } = new();
        public List<string> CorrectHeaders { get; set; } = new();
    }
}