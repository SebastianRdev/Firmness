namespace Firmness.WebAdmin.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firmness.WebAdmin.Models;

/// <summary>
/// Controller for the main dashboard view.
/// </summary>
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    // En el futuro podrás inyectar servicios o repositorios aquí
    // private readonly IProductRepository _productRepository;
    // private readonly ICustomerRepository _customerRepository;
    // private readonly ISalesRepository _salesRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardController"/> class.
    /// </summary>
    public DashboardController()
    {
        // Los repositorios se inyectarán más adelante
    }

    /// <summary>
    /// Displays the dashboard index view with summary data.
    /// </summary>
    /// <returns>The dashboard view.</returns>
    public IActionResult Index()
    {
        // Datos temporales (mock)
        var model = new DashboardViewModel
        {
            TotalProducts = 0,
            TotalCustomers = 0,
            TotalSales = 0
        };

        return View(model);
    }
}