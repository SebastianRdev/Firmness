using Firmness.Application.Interfaces;
using Firmness.Application.DTOs.Excel;
using Firmness.Application.Common;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Firmness.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly IExcelService _excelService;
    private readonly IGenericRepository<Customer> _customerRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly ILogger<ImportService> _logger;

    public ImportService(
        IExcelService excelService,
        IGenericRepository<Customer> customerRepo,
        IGenericRepository<Product> productRepo,
        ILogger<ImportService> logger)
    {
        _excelService = excelService;
        _customerRepo = customerRepo;
        _productRepo = productRepo;
        _logger = logger;
    }

    public async Task<ResultOft<BulkPreviewResultDto>> ProcessExcelPreviewAsync(IFormFile file, string entityType)
    {
        try
        {
            _logger.LogInformation("Processing Excel preview for entity: {EntityType}", entityType);

            if (file == null || file.Length == 0)
                return ResultOft<BulkPreviewResultDto>.Failure("El archivo está vacío");

            // 1. Extraer headers del Excel
            var headersResult = await _excelService.ExtractHeadersFromExcelAsync(file, entityType);
            
            if (!headersResult.IsSuccess)
                return ResultOft<BulkPreviewResultDto>.Failure(headersResult.ErrorMessage);

            var originalHeaders = headersResult.Data.OriginalHeaders;
            var correctHeaders = headersResult.Data.CorrectHeaders;

            // 2. Corregir headers con IA
            var correctionResult = await _excelService.CorrectColumnNamesAsync(originalHeaders, correctHeaders);
            
            if (!correctionResult.IsSuccess)
                return ResultOft<BulkPreviewResultDto>.Failure(correctionResult.ErrorMessage);

            var correctedHeaders = correctionResult.Data.CorrectedColumns;

            // 3. Generar preview con validación
            using var stream = file.OpenReadStream();
            var preview = await _excelService.GeneratePreviewAsync(stream, entityType, correctedHeaders);

            _logger.LogInformation(
                "Preview generated: {Valid} valid rows, {Invalid} invalid rows",
                preview.TotalValid, preview.TotalInvalid);

            return ResultOft<BulkPreviewResultDto>.Success(preview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Excel preview");
            return ResultOft<BulkPreviewResultDto>.Failure($"Error al procesar el archivo: {ex.Message}");
        }
    }

    public async Task<ResultOft<BulkInsertResultDto>> ConfirmImportAsync(BulkPreviewResultDto previewModel)
    {
        try
        {
            _logger.LogInformation("Confirming import with {Count} valid rows", previewModel.TotalValid);

            if (previewModel.ValidRows == null || !previewModel.ValidRows.Any())
            {
                return ResultOft<BulkInsertResultDto>.Success(new BulkInsertResultDto
                {
                    InsertedCount = 0,
                    FailedRows = new List<int>()
                });
            }

            // Determinar tipo de entidad basado en las propiedades de la primera fila
            var firstRow = previewModel.ValidRows.First().RowData;
            string entityType = DetermineEntityType(firstRow);

            int insertedCount = 0;
            var failedRows = new List<int>();

            switch (entityType.ToLower())
            {
                case "customer":
                    (insertedCount, failedRows) = await InsertCustomersAsync(previewModel.ValidRows);
                    break;

                case "product":
                    (insertedCount, failedRows) = await InsertProductsAsync(previewModel.ValidRows);
                    break;

                default:
                    return ResultOft<BulkInsertResultDto>.Failure($"Tipo de entidad no soportado: {entityType}");
            }

            _logger.LogInformation("Import completed: {Inserted} inserted, {Failed} failed", 
                insertedCount, failedRows.Count);

            return ResultOft<BulkInsertResultDto>.Success(new BulkInsertResultDto
            {
                InsertedCount = insertedCount,
                FailedRows = failedRows
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming import");
            return ResultOft<BulkInsertResultDto>.Failure($"Error al confirmar la importación: {ex.Message}");
        }
    }

    private string DetermineEntityType(Dictionary<string, string> rowData)
    {
        // Detectar tipo de entidad basado en las claves del diccionario
        if (rowData.ContainsKey("Username") || rowData.ContainsKey("FullName"))
            return "Customer";
        
        if (rowData.ContainsKey("Name") && rowData.ContainsKey("Price"))
            return "Product";

        return "Unknown";
    }

    private async Task<(int inserted, List<int> failed)> InsertCustomersAsync(List<RowValidationResultDto> validRows)
    {
        var customers = new List<Customer>();
        var failedRows = new List<int>();

        foreach (var row in validRows)
        {
            try
            {
                var customer = new Customer
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = row.RowData.GetValueOrDefault("Username", ""),
                    FullName = row.RowData.GetValueOrDefault("FullName", ""),
                    Email = row.RowData.GetValueOrDefault("Email", ""),
                    Address = row.RowData.GetValueOrDefault("Address", ""),
                    PhoneNumber = row.RowData.GetValueOrDefault("Phone", ""),
                    CreatedAt = DateTime.UtcNow
                };

                customers.Add(customer);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to map row {RowNumber}", row.RowNumber);
                failedRows.Add(row.RowNumber);
            }
        }

        if (customers.Any())
        {
            await _customerRepo.AddRangeAsync(customers);
            await _customerRepo.SaveChangesAsync();
        }

        return (customers.Count, failedRows);
    }

    private async Task<(int inserted, List<int> failed)> InsertProductsAsync(List<RowValidationResultDto> validRows)
    {
        var products = new List<Product>();
        var failedRows = new List<int>();

        foreach (var row in validRows)
        {
            try
            {
                var product = new Product
                {
                    Name = row.RowData.GetValueOrDefault("Name", ""),
                    Price = decimal.TryParse(row.RowData.GetValueOrDefault("Price", "0"), out var price) ? price : 0,
                    CategoryId = int.TryParse(row.RowData.GetValueOrDefault("CategoryId", "1"), out var catId) ? catId : 1,
                    Stock = int.TryParse(row.RowData.GetValueOrDefault("Stock", "0"), out var stock) ? stock : 0,
                    CreatedAt = DateTime.UtcNow
                };

                products.Add(product);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to map row {RowNumber}", row.RowNumber);
                failedRows.Add(row.RowNumber);
            }
        }

        if (products.Any())
        {
            await _productRepo.AddRangeAsync(products);
            await _productRepo.SaveChangesAsync();
        }

        return (products.Count, failedRows);
    }
}

