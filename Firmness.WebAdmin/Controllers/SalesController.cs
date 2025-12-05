namespace Firmness.WebAdmin.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing sales in the Web Admin interface.
/// </summary>
public class SalesController : Controller
{
    /// <summary>
    /// Displays the sales index view.
    /// </summary>
    /// <returns>The index view.</returns>
    // GET
    public IActionResult Index()
    {
        return View();
    }
}