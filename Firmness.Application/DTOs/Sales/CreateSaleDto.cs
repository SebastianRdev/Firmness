namespace Firmness.Application.DTOs.Sales;

public class CreateSaleDto
{
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public List<CreateSaleDetailDto> SaleDetails { get; set; } = new();
}

public class CreateSaleDetailDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
