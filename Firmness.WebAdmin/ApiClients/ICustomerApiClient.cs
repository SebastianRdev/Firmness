namespace Firmness.WebAdmin.ApiClients;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;

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
}