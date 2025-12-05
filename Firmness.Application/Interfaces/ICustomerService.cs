namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Customers;
using Firmness.Application.Common;
using Microsoft.AspNetCore.Http;
using Firmness.Application.DTOs.Excel;

public interface ICustomerService
{
    /// <summary>
    /// Retrieves a paginated list of all customers in the system.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A result containing a collection of customer DTOs.</returns>
    Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync(int page, int pageSize);

    /// <summary>
    /// Retrieves a customer by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>A result containing the customer DTO if found.</returns>
    Task<ResultOft<CustomerDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="createDto">The data transfer object containing customer creation details.</param>
    /// <returns>A result containing the created customer DTO.</returns>
    Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="updateDto">The data transfer object containing customer update details.</param>
    /// <returns>A result containing the updated customer DTO.</returns>
    Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto);

    /// <summary>
    /// Deletes a customer by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> DeleteAsync(Guid id);

    /// <summary>
    /// Searches for customers matching a given search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <returns>A result containing a collection of matching customer DTOs.</returns>
    Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Checks if a customer exists by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>True if the customer exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Retrieves the roles assigned to a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of role names.</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);

    /// <summary>
    /// Retrieves all available roles in the system.
    /// </summary>
    /// <returns>A collection of all role names.</returns>
    Task<IEnumerable<string>> GetAllRolesAsync();

    /// <summary>
    /// Updates the role of a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="newRole">The new role to assign to the user.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserRoleAsync(Guid userId, string newRole);

    /// <summary>
    /// Imports customers from an Excel file.
    /// </summary>
    /// <param name="file">The Excel file containing customer data.</param>
    /// <param name="entityType">The type of entity being imported (e.g., "Customer").</param>
    /// <returns>A result indicating the success or failure of the import operation.</returns>
    Task<Result> ImportFromExcelAsync(IFormFile file, string entityType);

    /// <summary>
    /// Extracts headers from an uploaded Excel file.
    /// </summary>
    /// <param name="file">The Excel file to extract headers from.</param>
    /// <returns>A result containing the extracted headers.</returns>
    Task<ResultOft<ExcelHeadersResponseDto>> ExtractHeadersFromExcelAsync(IFormFile file);
}