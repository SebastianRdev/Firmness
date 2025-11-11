namespace Firmness.WebAdmin.ApiClients;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Categories;

public interface ICategoryApiClient
{
    Task<ResultOft<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ResultOft<CategoryDto>> GetByIdAsync(int id);
    Task<ResultOft<CategoryDto>> CreateAsync(CreateCategoryDto createDto);
    Task<ResultOft<CategoryDto>> UpdateAsync(UpdateCategoryDto updateDto);
    Task<Result> DeleteAsync(int id);
    Task<ResultOft<IEnumerable<CategoryDto>>> SearchAsync(string searchTerm);
}