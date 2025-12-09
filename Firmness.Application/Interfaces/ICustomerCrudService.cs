using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;

namespace Firmness.Application.Interfaces;

/// <summary>
/// Service interface for Customer CRUD operations
/// </summary>
public interface ICustomerCrudService
{
    /// <summary>
    /// Retrieves paginated customers
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync(int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Retrieves a customer by their unique identifier
    /// </summary>
    Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Creates a new customer
    /// </summary>
    Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto);
    
    /// <summary>
    /// Updates an existing customer
    /// </summary>
    Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto);
    
    /// <summary>
    /// Deletes a customer
    /// </summary>
    Task<Result> DeleteAsync(Guid id);
    
    /// <summary>
    /// Searches for customers by term
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm);
    
    /// <summary>
    /// Checks if a customer exists
    /// </summary>
    Task<bool> ExistsAsync(Guid id);
}
