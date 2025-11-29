namespace Firmness.API.Controllers;

using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

public class SalesController : Controller
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale(Sale sale)
    {
        // Lógica de negocio para crear la venta
        await _saleService.CreateSaleAsync(sale);

        // Redirigir al usuario a la página de descarga del recibo
        return RedirectToAction("DownloadReceipt", new { fileName = sale.ReceiptFileName });
    }

    [HttpGet]
    public IActionResult DownloadReceipt(string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recibos", fileName);
        return PhysicalFile(filePath, "application/pdf", fileName);
    }
}

