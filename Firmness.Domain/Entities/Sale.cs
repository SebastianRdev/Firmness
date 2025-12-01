namespace Firmness.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string CustomerId { get; set; }
    public Customer Customer { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DeliveryFees { get; set; }
    public decimal GrandTotal { get; set; }
    public string ReceiptFileName { get; set; }
    public ICollection<SaleDetail> SaleDetails { get; set; }
    public Receipt Receipt { get; set; }
}
