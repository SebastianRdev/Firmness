namespace Firmness.WebAdmin.Models.Categories;

using Firmness.Application.DTOs.Categories;

/// <summary>
/// ViewModel for displaying category details.
/// </summary>
public class CategoryViewModel
{
    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of subcategories or related categories.
    /// </summary>
    public IEnumerable<CategoryDto>? Categories { get; set; }
}