namespace Firmness.WebAdmin.Models.Categories;

/// <summary>
/// ViewModel for creating a new category.
/// </summary>
public class CreateCategoryViewModel
{
    /// <summary>
    /// Gets or sets the name of the new category.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}