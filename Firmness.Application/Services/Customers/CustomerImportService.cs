namespace Firmness.Application.Services.Customers;

using Firmness.Application.Common;
using Firmness.Application.Configuration;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.DTOs.Excel;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

/// <summary>
/// Service for handling Customer Excel import operations
/// </summary>
public class CustomerImportService : ICustomerImportService
{
    private readonly IExcelService _excelService;
    private readonly ICustomerCrudService _customerCrudService;
    private readonly ILogger<CustomerImportService> _logger;

    public CustomerImportService(
        IExcelService excelService,
        ICustomerCrudService customerCrudService,
        ILogger<CustomerImportService> logger)
    {
        _excelService = excelService;
        _customerCrudService = customerCrudService;
        _logger = logger;
    }

    public async Task<Result> ImportFromExcelAsync(IFormFile file, string entityType)
    {
        try
        {
            using var package = new ExcelPackage(file.OpenReadStream());
            var worksheet = package.Workbook.Worksheets[0];

            int rowCount = worksheet.Dimension.Rows;
            int columnCount = worksheet.Dimension.Columns;

            // 1. Read original headers
            var headers = new List<string>();
            for (int col = 1; col <= columnCount; col++)
            {
                headers.Add(worksheet.Cells[1, col].Text.Trim());
            }

            // 2. Correct headers with AI
            var correction = await _excelService.CorrectColumnNamesAsync(
                headers,
                ColumnTemplates.Templates[entityType]
            );

            if (!correction.IsSuccess)
                return Result.Failure("Error correcting headers.");

            var corrected = correction.Data.CorrectedColumns;

            if (correction.Data.WasCorrected)
                _logger.LogInformation("Header corrections: {Report}", correction.Data.ChangesReport);

            // 3. Create header → index dictionary
            var headerIndex = corrected
                .Select((value, index) => new { value, index })
                .ToDictionary(x => x.value.ToLower(), x => x.index + 1);

            // 4. Counters
            int successCount = 0;
            var errors = new List<string>();

            // 5. Process rows
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var dto = new CreateCustomerDto
                    {
                        UserName = worksheet.Cells[row, headerIndex["username"]].Text,
                        FullName = worksheet.Cells[row, headerIndex["fullname"]].Text,
                        Email = worksheet.Cells[row, headerIndex["email"]].Text,
                        Address = worksheet.Cells[row, headerIndex["address"]].Text,
                        PhoneNumber = worksheet.Cells[row, headerIndex["phone"]].Text,
                        Password = "Temp123$" // Temporal password
                    };

                    if (string.IsNullOrWhiteSpace(dto.Email))
                    {
                        errors.Add($"Row {row}: empty email.");
                        continue;
                    }

                    var createResult = await _customerCrudService.CreateAsync(dto);

                    if (!createResult.IsSuccess)
                    {
                        errors.Add($"Row {row}: {createResult.ErrorMessage}");
                        continue;
                    }

                    successCount++;
                }
                catch (Exception exRow)
                {
                    errors.Add($"Row {row}: unexpected error: {exRow.Message}");
                }
            }

            _logger.LogInformation("Excel import completed: {Success} OK, {Errors} errors", successCount, errors.Count);

            if (errors.Any())
                return Result.Failure($"Imported {successCount} customers with {errors.Count} errors:\n" +
                                    string.Join("\n", errors));

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing Excel");
            return Result.Failure("Error processing the Excel file.");
        }
    }

    public async Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return ResultOft<ExcelHeadersResponseDto>.Failure("No file uploaded.");

            var headers = await _excelService.GetHeadersAsync(file);

            if (headers == null || headers.Count == 0)
                return ResultOft<ExcelHeadersResponseDto>.Failure("Excel contains no readable headers.");

            var dto = new ExcelHeadersResponseDto
            {
                OriginalHeaders = headers
            };

            return ResultOft<ExcelHeadersResponseDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting headers.");
            return ResultOft<ExcelHeadersResponseDto>.Failure("Error processing the Excel file.");
        }
    }
}
