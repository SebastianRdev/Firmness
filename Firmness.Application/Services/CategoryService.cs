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

    public async Task<ResultOft<CategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return ResultOft<CategoryDto>.Failure("The category ID must be greater than 0");
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            
            if (category == null)
            {
                _logger.LogWarning("Category with ID {{CategoryId}} not found", id);
                return ResultOft<CategoryDto>.Failure($"Category with ID {id} not found");
            }

            var dto = _mapper.Map<CategoryDto>(category);
            return ResultOft<CategoryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining the category {CategoryId}", id);
            return ResultOft<CategoryDto>.Failure("Error loading category. Please try again.");
        }
    }

    public async Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto)
    {
        try
        {
            var allCategories = await _categoryRepository.GetAllAsync();

            // Map and assign automatic values
            var category = _mapper.Map<Category>(createDto);

            // Save
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            // Return succesfully result
            var dto = _mapper.Map<CategoryDto>(category);
            _logger.LogInformation("Category '{CategoryName}' created with ID {CategoryId}", category.Name, category.Id);
            return ResultOft<CategoryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return ResultOft<CategoryDto>.Failure("Error creating category. Please try again.");
        }
    }

    public async Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto)
    {
        try
        {
            if (updateDto.Id <= 0)
            {
                return ResultOft<CategoryDto>.Failure("The category ID must be greater than 0");
            }
            
            var category = await _categoryRepository.GetByIdAsync(updateDto.Id);
            if (category == null)
            {
                _logger.LogWarning("Attempt to update non-existent category with ID {CategoryId}", updateDto.Id);
                return ResultOft<CategoryDto>.Failure($"Category with ID {updateDto.Id} not found");
            }

            // Map changes
            _mapper.Map(updateDto, category);

            // Save
            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            // Return result
            var dto = _mapper.Map<CategoryDto>(category);
            _logger.LogInformation("Updated '{CategoryName}' category (ID: {CategoryId})", category.Name, category.Id);
            return ResultOft<CategoryDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Category update failed {CategoryId}", updateDto.Id);
            return ResultOft<CategoryDto>.Failure("Category update failed. Please try again.");
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("The category ID must be greater than 0");
            }
            
            var exists = await _categoryRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Attempt to delete non-existent category with ID {CategoryId}", id);
                return Result.Failure($"Category with ID {id} not found");
            }

            // Delete
            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();

            _logger.LogInformation("Category with ID {CategoryId} removed", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting the category {CategoryId}", id);
            return Result.Failure("Error deleting the category. Please try again..");
        }
    }

    public async Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return ResultOft<IEnumerable<CategoryDto>>.Failure("The search term cannot be empty.");
            }

            if (searchTerm.Length < 2)
            {
                return ResultOft<IEnumerable<CategoryDto>>.Failure("The search term must be at least 2 characters long");
            }

            var categories = await _categoryRepository.GetAllAsync();
            
            var filtered = categories.Where(p => 
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );

            var dtos = _mapper.Map<IEnumerable<CategoryDto>>(filtered);
            _logger.LogInformation("Searching for categories with the term '{SearchTerm}' returned {Count} results", searchTerm, dtos.Count());
            return ResultOft<IEnumerable<CategoryDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for categories with term '{SearchTerm}'", searchTerm);
            return ResultOft<IEnumerable<CategoryDto>>.Failure("Error searching for categories. Please try again.");
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _categoryRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A category with the code already exists. {CategoryId}", id);
            return false;
        }
    }
}