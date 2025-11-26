namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
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
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
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
    
    [HttpPost("import-excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file selected.");
        }

        var result = await _customerService.ImportFromExcelAsync(file);
    
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(new { message = "Data imported successfully." });
    }
    
    [HttpPost("import/headers")]
    [ProducesResponseType(typeof(ExcelHeadersResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return ResultOft<ExcelHeadersResponseDto>.Failure("File is empty");

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
                return ResultOft<ExcelHeadersResponseDto>.Failure("No worksheet found");

            var headers = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[1, col].Text;
                headers.Add(header);
            }

            var dto = new ExcelHeadersResponseDto
            {
                OriginalHeaders = headers
            };

            return ResultOft<ExcelHeadersResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ResultOft<ExcelHeadersResponseDto>.Failure("Error extracting headers: " + ex.Message);
        }
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