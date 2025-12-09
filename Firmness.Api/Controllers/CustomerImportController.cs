namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;

/// <summary>
/// Manages customer Excel import operations
/// </summary>
[ApiController]
[Route("api/customers/import")]
// [Authorize(Roles = "Admin")]
public class CustomerImportController : ControllerBase
{
    private readonly IExcelService _excelService;
    private readonly ICustomerImportService _customerImportService;
    private readonly ILogger<CustomerImportController> _logger;

    public CustomerImportController(
        IExcelService excelService,
        ICustomerImportService customerImportService,
        ILogger<CustomerImportController> logger)
    {
        _excelService = excelService;
        _customerImportService = customerImportService;
        _logger = logger;
    }

    /// <summary>
    /// Extracts headers from uploaded Excel file
    /// </summary>
    [HttpPost("headers")]
    public async Task<IActionResult> GetHeaders([FromForm] IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            var result = await _customerImportService.ExtractHeadersFromExcelAsync(file);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting headers from Excel");
            return StatusCode(500, new { error = "Error processing Excel file" });
        }
    }

    /// <summary>
    /// Corrects Excel column headers using AI
    /// </summary>
    [HttpPost("correct-headers")]
    public async Task<IActionResult> CorrectHeaders([FromBody] CorrectHeadersRequest request)
    {
        try
        {
            if (request == null || request.RealColumns == null || !request.RealColumns.Any())
                return BadRequest(new { error = "Real columns are required" });

            if (request.CorrectColumns == null || !request.CorrectColumns.Any())
                return BadRequest(new { error = "Correct columns template is required" });

            var result = await _excelService.CorrectColumnNamesAsync(
                request.RealColumns,
                request.CorrectColumns
            );

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error correcting headers");
            return StatusCode(500, new { error = "Error processing headers" });
        }
    }

    /// <summary>
    /// Imports customers from Excel file
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ImportCustomers([FromForm] IFormFile file, [FromForm] string entityType = "customer")
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            if (string.IsNullOrEmpty(entityType))
                return BadRequest(new { error = "Entity type is required" });

            var result = await _customerImportService.ImportFromExcelAsync(file, entityType);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.ErrorMessage });

            return Ok(new { message = "Customers imported successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing customers from Excel");
            return StatusCode(500, new { error = "Error importing customers" });
        }
    }

    #region DTOs

    public record CorrectHeadersRequest(List<string> RealColumns, List<string> CorrectColumns);

    #endregion
}
