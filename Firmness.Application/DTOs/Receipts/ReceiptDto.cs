namespace Firmness.Application.DTOs.Receipts;

public class ReceiptDto
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string FileName { get; set; } = string.Empty;
}
