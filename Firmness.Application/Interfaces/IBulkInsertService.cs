namespace Firmness.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Excel;

public interface IBulkInsertService
{
    Task<BulkInsertResultDto> InsertAsync(
        string entityType,
        List<RowValidationResultDto> validRows);
}