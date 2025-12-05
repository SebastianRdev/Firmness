namespace Firmness.WebAdmin.Models;

/// <summary>
/// ViewModel for the dashboard, containing summary statistics.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Gets or sets the total number of products.
    /// </summary>
    public int TotalProducts { get; set; }

    /// <summary>
    /// Gets or sets the total number of customers.
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Gets or sets the total sales amount.
    /// </summary>
    public decimal TotalSales { get; set; }
}