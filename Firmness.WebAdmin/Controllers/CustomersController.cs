using Microsoft.AspNetCore.Mvc;

namespace Firmness.WebAdmin.Controllers;

public class CustomersController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}