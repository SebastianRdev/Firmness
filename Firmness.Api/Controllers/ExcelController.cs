using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers
{
    /// <summary>
    /// Controller responsible for Excel file processing and import operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelController : ControllerBase
    {
        private readonly IImportService _importService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelController"/> class.
        /// </summary>
        /// <param name="importService">The import service.</param>
        public ExcelController(IImportService importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Generates a preview of the Excel import.
        /// </summary>
        /// <param name="file">The Excel file to upload.</param>
        /// <param name="entityType">The type of entity to import.</param>
        /// <returns>A preview of the import data.</returns>
        /// <response code="200">Returns the preview data.</response>
        /// <response code="400">If the file is empty or entity type is missing.</response>
        [HttpPost("preview")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Confirms the Excel import and processes the data.
        /// </summary>
        /// <param name="previewModel">The preview data to confirm.</param>
        /// <returns>The result of the import operation.</returns>
        /// <response code="200">Returns the import result.</response>
        /// <response code="400">If the preview model is missing or import fails.</response>
        [HttpPost("confirm")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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
