using Firmness.Application.DTOs.Categories;

namespace Firmness.Application.Services;

using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Customers;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides business logic operations for managing customers,
/// including creation, retrieval, updating, deletion, and searching.
/// </summary>

public class CustomerService : ICustomerService
{
    private readonly IGenericRepository<ApplicationUser> _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;
    
    
    public Task<ResultOft<IEnumerable<CustomerDto>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<CustomerDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<CustomerDto>> CreateAsync(CreateCustomerDto createDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<CustomerDto>> UpdateAsync(UpdateCustomerDto updateDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<IEnumerable<CustomerDto>>> SearchAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }
}