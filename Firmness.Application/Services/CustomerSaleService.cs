namespace Firmness.Application.Services;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Sales;
using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;
using Firmness.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class CustomerSaleService : ICustomerSaleService
{
    private readonly IGenericRepository<Customer> _customerRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Sale> _saleRepository;
    private readonly IGenericRepository<Receipt> _receiptRepository;
    private readonly IReceiptPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IWebHostEnvironment _environment;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<CustomerSaleService> _logger;

    public CustomerSaleService(
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Product> productRepository,
        IGenericRepository<Sale> saleRepository,
        IGenericRepository<Receipt> receiptRepository,
        IReceiptPdfService pdfService,
        IEmailService emailService,
        IWebHostEnvironment environment,
        IOptions<EmailSettings> emailSettings,
        ILogger<CustomerSaleService> logger)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _saleRepository = saleRepository;
        _receiptRepository = receiptRepository;
        _pdfService = pdfService;
        _emailService = emailService;
        _environment = environment;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task<ResultOft<SaleResponseDto>> CreateSaleWithReceiptAsync(CreateSaleDto createDto)
    {
        try
        {
            // 1. Validate customer exists
            var customerIdStr = createDto.CustomerId.ToString();
            var customer = await _customerRepository.FirstOrDefaultAsync(c => c.Id == customerIdStr);
            if (customer == null)
            {
                return ResultOft<SaleResponseDto>.Failure("Customer not found");
            }

            // 2. Validate inventory and get products
            var validationResult = await ValidateInventoryAsync(createDto.SaleDetails);
            if (!validationResult.IsSuccess)
            {
                return ResultOft<SaleResponseDto>.Failure(validationResult.ErrorMessage);
            }

            // 3. Create Sale entity
            var sale = new Sale
            {
                CustomerId = customerIdStr,
                Date = DateTime.UtcNow,
                TotalAmount = createDto.TotalAmount,
                TaxAmount = createDto.TaxAmount,
                GrandTotal = createDto.GrandTotal,
                DeliveryFees = 0,
                SaleDetails = new List<SaleDetail>(),
                ReceiptFileName = "PENDING" // Initialize required field
            };

            // 4. Add sale details and update inventory
            foreach (var detailDto in createDto.SaleDetails)
            {
                var product = await _productRepository.GetByIdAsync(detailDto.ProductId);
                if (product == null) continue;

                // Update product stock
                product.Stock -= detailDto.Quantity;
                await _productRepository.UpdateAsync(product);

                sale.SaleDetails.Add(new SaleDetail
                {
                    ProductId = detailDto.ProductId,
                    Quantity = detailDto.Quantity,
                    UnitPrice = detailDto.UnitPrice,
                    Product = product
                });
            }

            // 5. Save sale
            await _saleRepository.AddAsync(sale);
            await _saleRepository.SaveChangesAsync();

            // 6. Create receipt
            var receipt = new Receipt
            {
                SaleId = sale.Id,
                ReceiptNumber = GenerateReceiptNumber(sale.Id),
                GeneratedAt = DateTime.UtcNow,
                FileName = $"Receipt_{sale.Id}.pdf"
            };
            receipt.FilePath = Path.Combine("receipts", receipt.FileName);

            await _receiptRepository.AddAsync(receipt);
            await _receiptRepository.SaveChangesAsync();
            
            sale.Receipt = receipt;
            sale.ReceiptFileName = receipt.FileName; // Update with actual filename
            await _saleRepository.UpdateAsync(sale); // Save the update


            // 7. Generate PDF
            var pdfPath = Path.Combine(_environment.WebRootPath, "receipts", receipt.FileName);
            var pdfGenerated = await _pdfService.GeneratePdfAsync(sale, pdfPath);

            if (!pdfGenerated)
            {
                _logger.LogWarning("PDF generation failed for sale {SaleId}", sale.Id);
            }

            // 8. Send emails (to customer and admin)
            try
            {
                var emailBody = GenerateEmailBody(sale, customer);
                var recipients = new List<string> { customer.Email };
                
                if (!string.IsNullOrEmpty(_emailSettings.AdminEmail))
                {
                    recipients.Add(_emailSettings.AdminEmail);
                }

                await _emailService.SendEmailToMultipleAsync(
                    recipients,
                    $"Comprobante de Venta #{sale.Id}",
                    emailBody,
                    pdfPath,
                    receipt.FileName
                );

                _logger.LogInformation("Receipt email sent successfully for sale {SaleId}", sale.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send receipt email for sale {SaleId}", sale.Id);
                // Don't fail the entire operation if email fails
            }

            // 9. Return response
            var response = new SaleResponseDto
            {
                Id = sale.Id,
                CustomerId = Guid.Parse(sale.CustomerId),
                CustomerName = customer.FullName,
                CustomerEmail = customer.Email,
                Date = sale.Date,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                GrandTotal = sale.GrandTotal,
                ReceiptFileName = receipt.FileName,
                SaleDetails = sale.SaleDetails.Select(d => new SaleDetailDto
                {
                    Id = d.Id,
                    ProductName = d.Product?.Name ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };

            return ResultOft<SaleResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sale with receipt");
            var errorMessage = ex.InnerException != null ? $"{ex.Message} Inner: {ex.InnerException.Message}" : ex.Message;
            return ResultOft<SaleResponseDto>.Failure($"Error creating sale: {errorMessage}");
        }
    }

    public async Task<ResultOft<SaleResponseDto>> GetSaleByIdAsync(int id)
    {
        try
        {
            var sale = await _saleRepository.GetByIdAsync(id, 
                s => s.Customer, 
                s => s.Receipt, 
                s => s.SaleDetails);

            if (sale == null)
            {
                return ResultOft<SaleResponseDto>.Failure("Sale not found");
            }

            var response = new SaleResponseDto
            {
                Id = sale.Id,
                CustomerId = Guid.Parse(sale.CustomerId),
                CustomerName = sale.Customer?.FullName ?? "Unknown",
                CustomerEmail = sale.Customer?.Email ?? "Unknown",
                Date = sale.Date,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                GrandTotal = sale.GrandTotal,
                ReceiptFileName = sale.Receipt?.FileName,
                SaleDetails = sale.SaleDetails?.Select(d => new SaleDetailDto
                {
                    Id = d.Id,
                    ProductName = d.Product?.Name ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList() ?? new List<SaleDetailDto>()
            };

            return ResultOft<SaleResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sale {SaleId}", id);
            return ResultOft<SaleResponseDto>.Failure($"Error retrieving sale: {ex.Message}");
        }
    }

    private async Task<Result> ValidateInventoryAsync(List<CreateSaleDetailDto> saleDetails)
    {
        foreach (var detail in saleDetails)
        {
            var product = await _productRepository.GetByIdAsync(detail.ProductId);
            
            if (product == null)
            {
                return Result.Failure($"Product with ID {detail.ProductId} not found");
            }

            if (product.Stock < detail.Quantity)
            {
                return Result.Failure($"Insufficient stock for product '{product.Name}'. Available: {product.Stock}, Requested: {detail.Quantity}");
            }
        }

        return Result.Success();
    }

    private string GenerateReceiptNumber(int saleId)
    {
        var date = DateTime.Now;
        return $"REC-{date:yyyyMMdd}-{saleId:D6}";
    }

    private string GenerateEmailBody(Sale sale, Customer customer)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #1890ff; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background-color: #f5f7fa; }}
                    .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                    .highlight {{ color: #1890ff; font-weight: bold; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>FIRMNESS - ConstructGo</h1>
                        <p>Comprobante de Venta</p>
                    </div>
                    <div class='content'>
                        <h2>Estimado/a {customer.FullName},</h2>
                        <p>Gracias por su compra. Adjunto encontrará el comprobante de su venta.</p>
                        
                        <p><strong>Detalles de la venta:</strong></p>
                        <ul>
                            <li>Número de venta: <span class='highlight'>#{sale.Id}</span></li>
                            <li>Fecha: <span class='highlight'>{sale.Date:dd/MM/yyyy HH:mm}</span></li>
                            <li>Total: <span class='highlight'>${sale.GrandTotal:N2}</span></li>
                        </ul>
                        
                        <p>Si tiene alguna pregunta, no dude en contactarnos.</p>
                    </div>
                    <div class='footer'>
                        <p>Este es un correo automático, por favor no responder.</p>
                        <p>&copy; {DateTime.Now.Year} FIRMNESS - ConstructGo. Todos los derechos reservados.</p>
                    </div>
                </div>
            </body>
            </html>
        ";
    }
}