namespace Firmness.WebAdmin.ApiClients;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.DTOs.Excel;

public interface ICustomerApiClient
{
    Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync();
    Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id);
    Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto);
    Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto);
    Task<Result> DeleteAsync(Guid id);
    Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm);
    Task<ResultOft<IEnumerable<string>>> GetAllRolesAsync();
    Task<Result>UpdateUserRoleAsync(Guid id, string selectedRole);
    Task<ResultOft<IEnumerable<CustomerDto>>> GetAllPaginatedAsync(int page, int pageSize);
    Task<Result> ImportExcelAsync(IFormFile file);
    Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file, string entityType);
    Task<ResultOft<BulkInsertResultDto>> BulkInsertAsync(
        IFormFile file,
        string entityType,
        List<string> correctedHeaders
    );

    Task<ResultOft<ExcelHeadersResponseDto>> CorrectHeadersAsync(List<string> originalHeaders, List<string> correctHeaders);
}