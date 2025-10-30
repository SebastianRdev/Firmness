namespace Firmness.WebAdmin.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Firmness.WebAdmin.Models;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    // En el futuro podrás inyectar servicios o repositorios aquí
    // private readonly IProductRepository _productRepository;
    // private readonly ICustomerRepository _customerRepository;
    // private readonly ISalesRepository _salesRepository;

    public DashboardController()
    {
        // Los repositorios se inyectarán más adelante
    }

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