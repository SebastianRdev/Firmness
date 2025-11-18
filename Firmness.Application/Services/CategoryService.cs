namespace Firmness.Application.Services;

using AutoMapper;
using Firmness.Application.Common;
using Firmness.Application.DTOs.Categories;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides business logic operations for managing categories,
/// including creation, retrieval, updating, deletion, and searching.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoryService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoryService"/> class.
    /// </summary>
    /// <param name="categoryRepository">The repository used for data access of categories.</param>
    /// <param name="mapper">The AutoMapper instance used for object mapping.</param>
    /// <param name="logger">The logger used to record application events and errors.</param>
    public CategoryService(
        IGenericRepository<Category> categoryRepository,
        IMapper mapper,
        ILogger<CategoryService> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>
    /// A <see cref="ResultOft{T}"/> containing a collection of <see cref="CategoryDto"/> 
    /// if successful, or an error message if failed.
    /// </returns>
    public async Task<ResultOft<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ResultOft<IEnumerable<CategoryDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all categories");
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Error loading categories. Please try again.");
        }
    }

    public Task<ResultOft<CategoryDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }
}