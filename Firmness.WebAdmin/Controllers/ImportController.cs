using Microsoft.AspNetCore.Mvc;
using Firmness.WebAdmin.Models;
using Firmness.Application.Interfaces;

namespace Firmness.WebAdmin.Controllers;

public class ImportController : Controller
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost]
    public IActionResult Preview([FromBody] PreviewViewModel vm)
    {
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Confirm([FromBody] PreviewViewModel vm)
    {
        // This controller is deprecated - use ExcelController in API instead
        return BadRequest(new { message = "Please use /api/excel/preview and /api/excel/confirm endpoints" });
    }
}
