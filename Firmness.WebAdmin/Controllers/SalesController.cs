namespace Firmness.WebAdmin.Controllers;

using Microsoft.AspNetCore.Mvc;

public class SalesController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}