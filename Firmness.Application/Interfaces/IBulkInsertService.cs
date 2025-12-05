namespace Firmness.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;

public interface IBulkInsertService
{
    /// <summary>
    /// Inserts a list of valid rows into the database for a specific entity type.
    /// </summary>
    /// <param name="entityType">The type of entity to insert.</param>
    /// <param name="validRows">The list of valid rows to insert.</param>
    /// <returns>A result summary of the bulk insert operation.</returns>
    Task<BulkInsertResultDto> InsertAsync(
        string entityType,
        List<RowValidationResultDto> validRows);
}