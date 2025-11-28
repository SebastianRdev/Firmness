namespace Firmness.Domain.Entities;

public class Receipt
{
    public int Id { get; set; }  // Unique receipt ID
    public int SaleId { get; set; }  // Foreign key to Sale entity
    public Sale Sale { get; set; }  // Navigation property to Sale
    public string FileName { get; set; }  // Name of the PDF receipt file
    public string FilePath { get; set; }  // Full path to where the PDF is stored
    public DateTime GeneratedAt { get; set; }  // Timestamp when the receipt was generated
}
