using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.API.Controllers;

/// <summary>
/// Controller responsible for handling bulk data upload operations.
/// </summary>
[ApiController]
[Route("api/bulk")]
public class BulkController : ControllerBase
{
    private readonly IExcelService _excelService;
    private readonly IBulkInsertService _bulkInsertService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkController"/> class.
    /// </summary>
    /// <param name="excelService">The Excel service.</param>
    /// <param name="bulkInsertService">The bulk insert service.</param>
    public BulkController(IExcelService excelService, IBulkInsertService bulkInsertService)
    {
        _excelService = excelService;
        _bulkInsertService = bulkInsertService;
    }

    /// <summary>
    /// Generates a preview of the bulk upload from an Excel file.
    /// </summary>
    /// <param name="file">The Excel file to upload.</param>
    /// <param name="entityType">The type of entity to import (e.g., "customer", "product").</param>
    /// <param name="correctedHeaders">The list of corrected headers for mapping.</param>
    /// <returns>A preview of the valid and invalid rows.</returns>
    /// <response code="200">Returns the preview result.</response>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(BulkPreviewResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Preview([FromForm] IFormFile file, string entityType, [FromBody] List<string> correctedHeaders)
    {
        using var stream = file.OpenReadStream();

        var preview = await _excelService.GeneratePreviewAsync(
            stream, entityType, correctedHeaders
        );

        return Ok(preview);
    }

    /// <summary>
    /// Confirms the bulk upload and inserts the valid rows into the database.
    /// </summary>
    /// <param name="entityType">The type of entity to insert.</param>
    /// <param name="validRows">The list of valid rows to insert.</param>
    /// <returns>A result summary of the bulk insert operation.</returns>
    /// <response code="200">Returns the insert result.</response>
    [HttpPost("confirm")]
    [ProducesResponseType(typeof(BulkInsertResultDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Confirm(string entityType, [FromBody] List<RowValidationResultDto> validRows)
    {
        var result = await _bulkInsertService.InsertAsync(entityType, validRows);
        return Ok(result);
    }
}
