namespace Firmness.Application.Interfaces;

using Firmness.Domain.Entities;

public interface IReceiptPdfService
{
    /// <summary>
    /// Generates a PDF receipt for a sale and saves it to the specified path
    /// </summary>
    /// <param name="sale">The sale entity with all related data</param>
    /// <param name="outputPath">Full path where the PDF should be saved</param>
    /// <returns>True if successful, false otherwise</returns>
    Task<bool> GeneratePdfAsync(Sale sale, string outputPath);
}
