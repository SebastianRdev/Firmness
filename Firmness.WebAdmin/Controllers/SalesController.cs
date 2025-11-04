using Microsoft.AspNetCore.Mvc;

namespace Firmness.WebAdmin.Controllers;

public class SalesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}