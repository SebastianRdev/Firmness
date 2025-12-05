using Microsoft.AspNetCore.Mvc;
using Firmness.Web.Models;
using Firmness.Application.Interfaces;

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
        var result = await _importService.ImportAsync(vm.EntityType, vm.Rows);

        return Ok(new { message = "Importación completada con éxito" });
    }
}
