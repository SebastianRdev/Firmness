using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.API.Controllers;

[ApiController]
[Route("api/bulk")]
public class BulkController : ControllerBase
{
    private readonly IExcelService _excelService;
    private readonly IBulkInsertService _bulkInsertService;

    public BulkController(IExcelService excelService, IBulkInsertService bulkInsertService)
    {
        _excelService = excelService;
        _bulkInsertService = bulkInsertService;
    }

    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromForm] IFormFile file, string entityType, [FromBody] List<string> correctedHeaders)
    {
        using var stream = file.OpenReadStream();

        var preview = await _excelService.GeneratePreviewAsync(
            stream, entityType, correctedHeaders
        );

        return Ok(preview);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(string entityType, [FromBody] List<RowValidationResultDto> validRows)
    {
        var result = await _bulkInsertService.InsertAsync(entityType, validRows);
        return Ok(result);
    }
}
