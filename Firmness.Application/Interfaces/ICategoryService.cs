namespace Firmness.Application.Interfaces;

using Firmness.Application.DTOs.Categories;
using Firmness.Application.Common;

public interface ICategoryService
{
    /// <summary>
    /// Retrieves all categories in the system.
    /// </summary>
    /// <returns>A result containing a collection of category DTOs.</returns>
    Task<ResultOft<IEnumerable<CategoryDto>>> GetAllAsync();

    /// <summary>
    /// Retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>A result containing the category DTO if found.</returns>
    Task<ResultOft<CategoryDto>> GetByIdAsync(int id);

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="createDto">The data transfer object containing category creation details.</param>
    /// <returns>A result containing the created category DTO.</returns>
    Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="updateDto">The data transfer object containing category update details.</param>
    /// <returns>A result containing the updated category DTO.</returns>
    Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto);

    /// <summary>
    /// Deletes a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    Task<Result> DeleteAsync(int id);

    /// <summary>
    /// Searches for categories matching a given search term.
    /// </summary>
    /// <param name="searchTerm">The term to search for.</param>
    /// <returns>A result containing a collection of matching category DTOs.</returns>
    Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm);

    /// <summary>
    /// Checks if a category exists by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>True if the category exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(int id);
}