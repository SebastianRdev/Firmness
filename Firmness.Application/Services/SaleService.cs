namespace Firmness.Application.Services;

using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;

public class SaleService : ISaleService
{
    private readonly IGenericRepository<Sale> _saleRepository;
    private readonly ReceiptPdfService _receiptPdfService;

    public SaleService(IGenericRepository<Sale> saleRepository, ReceiptPdfService receiptPdfService)
    {
        _saleRepository = saleRepository;
        _receiptPdfService = receiptPdfService;
    }

    public async Task CreateSaleAsync(Sale sale)
    {
        // Guardar la venta en la base de datos
        await _saleRepository.AddAsync(sale);

        // Generar el PDF del recibo
        string receiptDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "recibos");
        var pdfPath = _receiptPdfService.GenerateReceiptPdf(sale, receiptDirectory);

        // Guardar el nombre del archivo en la base de datos
        sale.ReceiptFileName = Path.GetFileName(pdfPath);
        await _saleRepository.AddAsync(sale); // Actualizamos la venta con el nombre del archivo
    }

    public async Task<Sale> GetSaleByIdAsync(int id)
    {
        return await _saleRepository.GetByIdAsync(id);
    }
}