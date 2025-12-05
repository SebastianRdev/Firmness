using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IImportService _importService;

        public ExcelController(IImportService importService)
        {
            _importService = importService;
        }

        [HttpPost("preview")]
        public async Task<IActionResult> Preview([FromForm] IFormFile file, [FromQuery] string entityType)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "Archivo vacío" });

            if (string.IsNullOrWhiteSpace(entityType))
                return BadRequest(new { success = false, message = "Tipo de entidad requerido" });

            var result = await _importService.ProcessExcelPreviewAsync(file, entityType);

            if (!result.IsSuccess)
                return BadRequest(new { success = false, message = result.ErrorMessage });

            return Ok(new { success = true, data = result.Data });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm([FromBody] BulkPreviewResultDto previewModel)
        {
            if (previewModel == null)
                return BadRequest(new { success = false, message = "Datos de preview requeridos" });

            var result = await _importService.ConfirmImportAsync(previewModel);

            if (!result.IsSuccess)
                return BadRequest(new { success = false, message = result.ErrorMessage });

            return Ok(new { success = true, data = result.Data });
        }
    }
}
