namespace Firmness.Application.DTOs.Categories;

using Firmness.Application.DTOs.Categories;

/// <summary>
/// Data transfer object for category details.
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subcategories, if any.
    /// </summary>
    public IEnumerable<CategoryDto>? Categories { get; set; }
}
