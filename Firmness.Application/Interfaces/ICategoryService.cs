namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Categories;
using Firmness.Application.Common;

public interface ICategoryService
{
    /// <summary>
    /// It obtains all the categories of the system.
    /// </summary>
    Task<ResultOft<IEnumerable<CategoryDto>>> GetAllAsync();

    /// <summary>
    /// You get a category by your ID.
    /// </summary>
    Task<ResultOft<CategoryDto>> GetByIdAsync(int id);

    /// <summary>
    /// Create a new category.
    /// </summary>
    Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto);

    /// <summary>
    /// Delete a category.
    /// </summary>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Search for categories by term.
    /// </summary>
    Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Check if a category exists.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}