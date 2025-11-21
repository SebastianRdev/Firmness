namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;

public interface ICustomerService
{
    /// <summary>
    /// It obtains all the customers of the system.
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync();

    /// <summary>
    /// You get a customer by your ID.
    /// </summary>
    Task<ResultOft<CustomerDto>> GetByIdAsync(int id);

    /// <summary>
    /// Create a new customer.
    /// </summary>
    Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto);

    /// <summary>
    /// Update an existing customer.
    /// </summary>
    Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto);

    /// <summary>
    /// Delete a customer.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Search for customers by term.
    /// </summary>
    Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Check if a customer exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}