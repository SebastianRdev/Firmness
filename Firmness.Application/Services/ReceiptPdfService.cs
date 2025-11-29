namespace Firmness.Application.Services;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Domain.Entities;
using System.IO;

public class ReceiptPdfService
{
    public string GenerateReceiptPdf(Sale sale, string receiptDirectory)
    {
        // Create a PDF document 
        var filePath = Path.Combine(receiptDirectory, $"Receipt_{sale.Id}.pdf");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(595, 842);
                page.Margin(30);
                
                page.Header().Text("Sales Receipt").SemiBold().FontSize(24).AlignCenter();
                
                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    // Sales details
                    column.Item().Text($"Date: {sale.Date:dd/MM/yyyy}");
                    column.Item().Text($"Sales number: {sale.Id}");
                    column.Item().Text($"Customer: {sale.Customer.FullName}");
                    column.Item().Text($"Email: {sale.Customer.Email}");
                    column.Item().Text($"Address: {sale.Customer.Address}");
                    
                    column.Spacing(20);
                    
                    // Products
                    column.Item().Text("Products", TextStyle.Default.Size(18).Bold());
                    foreach (var detail in sale.SaleDetails)
                    {
                        column.Item().Text($"{detail.Product.Name} - {detail.Quantity} x {detail.UnitPrice:C} = {detail.TotalPrice:C}");
                    }

                    column.Spacing(20);

                    // Totals
                    column.Item().Text($"Total: {sale.TotalAmount:C}");
                    column.Item().Text($"Tax: {sale.TaxAmount:C}");
                    column.Item().Text($"Total including taxes: {sale.GrandTotal:C}");
                });
            });
        });

        // Generate the PDF
        document.GeneratePdf(filePath);

        return filePath;  // Returns the full path of the generated PDF file
    }
}
