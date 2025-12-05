namespace Firmness.Application.Services;

using Firmness.Application.MappingTemplates;
using Firmness.Application.Validators;
using Microsoft.Extensions.Logging;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;
using OfficeOpenXml;

/// <summary>
/// Service responsible for handling Excel file operations, including reading, header extraction, and preview generation.
/// </summary>
public class ExcelService : IExcelService
{
    private readonly IGeminiService _geminiClient;
    private readonly ILogger<ExcelService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExcelService"/> class.
    /// </summary>
    /// <param name="geminiClient">The service for AI-powered column correction.</param>
    /// <param name="logger">The logger instance.</param>
    public ExcelService(
        IGeminiService geminiClient,
        ILogger<ExcelService> logger)
    {
        _geminiClient = geminiClient;
        _logger = logger;
    }

    /// <summary>
    /// Reads the headers from an Excel file.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <returns>A list of header names found in the first row.</returns>
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

    /// <summary>
    /// Reads rows from an Excel file into a list of dictionaries.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="startRow">The row number to start reading from (default is 2).</param>
    /// <returns>A list of dictionaries where keys are column names (e.g., "col1", "col2") and values are cell contents.</returns>
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

    /// <summary>
    /// Extracts headers from an Excel file and validates them against the expected template for the entity type.
    /// </summary>
    /// <param name="file">The uploaded Excel file.</param>
    /// <param name="entityType">The type of entity to validate headers against.</param>
    /// <returns>A result containing the extracted headers and potential corrections.</returns>
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

            // Read headers from Excel
            var realHeaders = new List<string>();
            for (int col = 1; col <= ws.Dimension.End.Column; col++)
            {
                var header = ws.Cells[1, col].Text?.Trim() ?? string.Empty;
                realHeaders.Add(header);
            }

            _logger.LogInformation("Read {Count} headers from Excel: {Headers}", 
                realHeaders.Count, string.Join(", ", realHeaders));

            // Find template
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

    /// <summary>
    /// Corrects column names using AI to match the expected template.
    /// </summary>
    /// <param name="realColumns">The actual column names found in the file.</param>
    /// <param name="correctColumns">The expected correct column names.</param>
    /// <returns>A result containing the corrected headers.</returns>
    public async Task<ResultOft<ExcelHeadersResponseDto>> CorrectColumnNamesAsync(
        List<string> realColumns,
        List<string> correctColumns)
    {
        try
        {
            // Call Gemini to get correction
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

    /// <summary>
    /// Generates a preview of the bulk insert operation by validating rows against the corrected headers.
    /// </summary>
    /// <param name="fileStream">The stream of the Excel file.</param>
    /// <param name="entityType">The type of entity being imported.</param>
    /// <param name="correctedHeaders">The list of corrected headers to use for mapping.</param>
    /// <returns>A result containing valid and invalid rows for preview.</returns>
    public async Task<BulkPreviewResultDto> GeneratePreviewAsync(
        Stream fileStream,
        string entityType,
        List<string> correctedHeaders)
    {
        //ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        ExcelPackage.License.SetNonCommercialOrganization("Firmness.Application");

        using var package = new ExcelPackage(fileStream);
        var sheet = package.Workbook.Worksheets.First();

        _logger.LogInformation("Reading preview rows for entity {EntityType}", entityType);

        // 1. Read rows with corrected headers
        var rawRows = EPPlusHelper.ReadRows(sheet, correctedHeaders);

        // 2. Map corrected headers -> real properties
        var mapping = entityType.ToLower() switch
        {
            "customer" => CustomerColumnTemplate.Map,
            "product"  => ProductColumnTemplate.Map,
            _ => throw new Exception($"No mapping template for entity: {entityType}")
        };

        // 3. Choose correct validator
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

            // 3.1 Map columns
            foreach (var kv in raw)
            {
                if (mapping.TryGetValue(kv.Key, out var propName))
                    mappedRow[propName] = kv.Value;
            }

            // 4. Validate row
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