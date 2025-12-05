using Microsoft.AspNetCore.Mvc;
using Firmness.WebAdmin.Models;
using Firmness.Application.Interfaces;

namespace Firmness.WebAdmin.Controllers;

/// <summary>
/// Controller for handling import operations (Deprecated).
/// </summary>
public class ImportController : Controller
{
    private readonly IImportService _importService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportController"/> class.
    /// </summary>
    /// <param name="importService">The import service.</param>
    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    /// <summary>
    /// Displays the preview of the import.
    /// </summary>
    /// <param name="vm">The preview view model.</param>
    /// <returns>The preview view.</returns>
    [HttpPost]
    public IActionResult Preview([FromBody] PreviewViewModel vm)
    {
        return View(vm);
    }

    /// <summary>
    /// Confirms the import (Deprecated).
    /// </summary>
    /// <param name="vm">The preview view model.</param>
    /// <returns>A bad request result indicating deprecation.</returns>
    [HttpPost]
    public async Task<IActionResult> Confirm([FromBody] PreviewViewModel vm)
    {
        // This controller is deprecated - use ExcelController in API instead
        return BadRequest(new { message = "Please use /api/excel/preview and /api/excel/confirm endpoints" });
    }
}
