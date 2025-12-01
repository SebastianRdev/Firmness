namespace Firmness.Application.Services;

using Firmness.Application.Interfaces;
using Firmness.Domain.Entities;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class ReceiptPdfService : IReceiptPdfService
{
    private readonly ILogger<ReceiptPdfService> _logger;

    public ReceiptPdfService(ILogger<ReceiptPdfService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> GeneratePdfAsync(Sale sale, string outputPath)
    {
        try
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await Task.Run(() =>
            {
                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        page.Header().Element(ComposeHeader);
                        page.Content().Element(content => ComposeContent(content, sale));
                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                }).GeneratePdf(outputPath);
            });

            _logger.LogInformation("PDF receipt generated successfully at {Path}", outputPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF receipt at {Path}", outputPath);
            return false;
        }
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("FIRMNESS - ConstructGo")
                    .FontSize(20)
                    .SemiBold()
                    .FontColor(Colors.Blue.Darken2);

                column.Item().Text("Sistema de Gestión de Inventario")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
            });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item().Text("COMPROBANTE DE VENTA")
                    .FontSize(16)
                    .SemiBold();
            });
        });
    }

    private void ComposeContent(IContainer container, Sale sale)
    {
        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);

            // Receipt Information
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Recibo N°: {sale.Receipt?.ReceiptNumber ?? "N/A"}")
                        .SemiBold();
                    col.Item().Text($"Fecha: {sale.Date:dd/MM/yyyy HH:mm}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Venta N°: {sale.Id}").SemiBold();
                });
            });

            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            // Customer Information
            column.Item().PaddingTop(10).Text("DATOS DEL CLIENTE").SemiBold().FontSize(12);
            column.Item().Row(row =>
            {
                row.RelativeItem().Text($"Nombre: {sale.Customer?.FullName ?? "N/A"}");
                row.RelativeItem().Text($"Email: {sale.Customer?.Email ?? "N/A"}");
            });

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            // Products Table
            column.Item().PaddingTop(10).Text("DETALLE DE PRODUCTOS").SemiBold().FontSize(12);
            
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);  // #
                    columns.RelativeColumn(3);    // Product
                    columns.RelativeColumn(1);    // Qty
                    columns.RelativeColumn(1.5f); // Unit Price
                    columns.RelativeColumn(1.5f); // Total
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Producto");
                    header.Cell().Element(CellStyle).AlignRight().Text("Cant.");
                    header.Cell().Element(CellStyle).AlignRight().Text("Precio Unit.");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten1)
                            .PaddingVertical(5)
                            .Background(Colors.Grey.Lighten3);
                    }
                });

                // Rows
                int index = 1;
                foreach (var detail in sale.SaleDetails ?? new List<SaleDetail>())
                {
                    table.Cell().Element(RowCellStyle).Text(index.ToString());
                    table.Cell().Element(RowCellStyle).Text(detail.Product?.Name ?? "N/A");
                    table.Cell().Element(RowCellStyle).AlignRight().Text(detail.Quantity.ToString());
                    table.Cell().Element(RowCellStyle).AlignRight().Text($"${detail.UnitPrice:N2}");
                    table.Cell().Element(RowCellStyle).AlignRight().Text($"${(detail.Quantity * detail.UnitPrice):N2}");
                    index++;
                }

                static IContainer RowCellStyle(IContainer container)
                {
                    return container
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5);
                }
            });

            // Totals
            column.Item().PaddingTop(20).AlignRight().Column(totalsColumn =>
            {
                totalsColumn.Item().Row(row =>
                {
                    row.ConstantItem(150).Text("Subtotal:");
                    row.ConstantItem(100).AlignRight().Text($"${sale.TotalAmount:N2}");
                });

                totalsColumn.Item().Row(row =>
                {
                    row.ConstantItem(150).Text("IVA:");
                    row.ConstantItem(100).AlignRight().Text($"${sale.TaxAmount:N2}");
                });

                totalsColumn.Item().Row(row =>
                {
                    row.ConstantItem(150).Text("Delivery:");
                    row.ConstantItem(100).AlignRight().Text($"${sale.DeliveryFees:N2}");
                });

                totalsColumn.Item().PaddingTop(5).LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                totalsColumn.Item().PaddingTop(5).Row(row =>
                {
                    row.ConstantItem(150).Text("TOTAL:").SemiBold().FontSize(14);
                    row.ConstantItem(100).AlignRight().Text($"${sale.GrandTotal:N2}")
                        .SemiBold()
                        .FontSize(14)
                        .FontColor(Colors.Blue.Darken2);
                });
            });

            // Footer note
            column.Item().PaddingTop(30).AlignCenter().Text("Gracias por su compra")
                .FontSize(10)
                .Italic()
                .FontColor(Colors.Grey.Darken1);
        });
    }
}
