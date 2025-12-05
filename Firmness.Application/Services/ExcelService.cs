namespace Firmness.Application.Services;

using Firmness.Application.MappingTemplates;
using Firmness.Application.Validators;
using Microsoft.Extensions.Logging;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;
using OfficeOpenXml;

public class ExcelService : IExcelService
{
    private readonly IGeminiService _geminiClient;
    private readonly ILogger<ExcelService> _logger;

    public ExcelService(
        IGeminiService geminiClient,
        ILogger<ExcelService> logger)
    {
        _geminiClient = geminiClient;
        _logger = logger;
    }

    public async Task<List<string>> GetHeadersAsync(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();

            int colCount = ws.Dimension?.End.Column ?? 0;
            var headers = new List<string>();

            for (int c = 1; c <= colCount; c++)
            {
                var header = ws.Cells[1, c].Text?.Trim() ?? string.Empty;
                headers.Add(header);
            }

            return headers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ExcelService] Error reading headers");
            throw;
        }
    }

    public async Task<List<Dictionary<string, string>>> ReadRowsAsync(IFormFile file, int startRow = 2)
    {
        using var stream = file.OpenReadStream();
        using var package = new ExcelPackage(stream);
        var ws = package.Workbook.Worksheets.First();

        int rows = ws.Dimension.End.Row;
        int cols = ws.Dimension.End.Column;

        var list = new List<Dictionary<string, string>>();

        for (int r = startRow; r <= rows; r++)
        {
            var dict = new Dictionary<string, string>();
            for (int c = 1; c <= cols; c++)
            {
                dict[$"col{c}"] = ws.Cells[r, c].Text?.Trim() ?? "";
            }
            list.Add(dict);
        }

        return list;
    }

    public async Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(
        IFormFile file, 
        string entityType)
    {
        try
        {
            _logger.LogInformation("Extracting headers from Excel file: {FileName}, EntityType: {EntityType}", 
                file.FileName, entityType);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is empty or null");
                return ResultOft<ExcelHeadersResponseDto>.Failure("File is empty");
            }

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.FirstOrDefault();
            
            if (ws == null)
            {
                _logger.LogWarning("No worksheet found in Excel file");
                return ResultOft<ExcelHeadersResponseDto>.Failure("No worksheet found");
            }

            if (ws.Dimension == null)
            {
                _logger.LogWarning("Worksheet is empty (no dimensions)");
                return ResultOft<ExcelHeadersResponseDto>.Failure("Worksheet is empty");
            }

            // Leer headers del Excel
            var realHeaders = new List<string>();
            for (int col = 1; col <= ws.Dimension.End.Column; col++)
            {
                var header = ws.Cells[1, col].Text?.Trim() ?? string.Empty;
                realHeaders.Add(header);
            }

            _logger.LogInformation("Read {Count} headers from Excel: {Headers}", 
                realHeaders.Count, string.Join(", ", realHeaders));

            // Buscar plantilla
            if (!ColumnTemplates.Templates.ContainsKey(entityType))
            {
                _logger.LogWarning("No template found for entity type: {EntityType}", entityType);
                return ResultOft<ExcelHeadersResponseDto>.Failure(
                    $"No column template found for entity '{entityType}'");
            }

            var correctHeaders = ColumnTemplates.Templates[entityType];

            _logger.LogInformation("Template headers: {Headers}", string.Join(", ", correctHeaders));

            var dto = new ExcelHeadersResponseDto
            {
                OriginalHeaders = realHeaders,
                CorrectHeaders = correctHeaders
            };

            return ResultOft<ExcelHeadersResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting headers from Excel");
            return ResultOft<ExcelHeadersResponseDto>.Failure($"Error extracting headers: {ex.Message}");
        }
    }

    public async Task<ResultOft<ExcelHeadersResponseDto>> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns)
    {
        try
        {
            // Llamar a Gemini para obtener la corrección
            var correction = await _geminiClient.CorrectColumnNamesAsync(realColumns, correctColumns);

            if (correction == null)
                return ResultOft<ExcelHeadersResponseDto>.Failure("AI returned no data");

            return ResultOft<ExcelHeadersResponseDto>.Success(correction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini AI");
            return ResultOft<ExcelHeadersResponseDto>.Failure("Error calling Gemini AI");
        }
    }

    public async Task<BulkPreviewResultDto> GeneratePreviewAsync(
        Stream fileStream,
        string entityType,
        List<string> correctedHeaders)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(fileStream);
        var sheet = package.Workbook.Worksheets.First();

        _logger.LogInformation("Reading preview rows for entity {EntityType}", entityType);

        // 1. Leer filas con los headers corregidos
        var rawRows = EPPlusHelper.ReadRows(sheet, correctedHeaders);

        // 2. Mapear headers corregidos → propiedades reales
        var mapping = entityType.ToLower() switch
        {
            "customer" => CustomerColumnTemplate.Map,
            "product"  => ProductColumnTemplate.Map,
            _ => throw new Exception($"No mapping template for entity: {entityType}")
        };

        // 3. Escoger validador correcto
        IExcelRowValidator validator = entityType.ToLower() switch
        {
            "customer" => new CustomerRowValidator(),
            "product"  => new ProductRowValidator(),
            _ => throw new Exception($"No validator for entity: {entityType}")
        };


        var preview = new BulkPreviewResultDto();
        int rowNumber = 2;

        foreach (var raw in rawRows)
        {
            var mappedRow = new Dictionary<string, string>();

            // 3.1 Mapear columnas
            foreach (var kv in raw)
            {
                if (mapping.TryGetValue(kv.Key, out var propName))
                    mappedRow[propName] = kv.Value;
            }

            // 4. Validar fila
            var validation = validator.Validate(mappedRow, rowNumber);

            if (validation.IsValid)
                preview.ValidRows.Add(validation);
            else
                preview.InvalidRows.Add(validation);

            rowNumber++;
        }

        _logger.LogInformation(
            "Preview generated: {Valid} valid rows, {Invalid} invalid rows",
            preview.TotalValid, preview.TotalInvalid
        );

        return preview;
    }
}