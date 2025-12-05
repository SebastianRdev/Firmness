namespace Firmness.WebAdmin.Models.Categories;

/// <summary>
/// ViewModel for editing an existing category.
/// </summary>
public class EditCategoryViewModel
{
    /// <summary>
    /// Gets or sets the ID of the category to edit.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the new name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}