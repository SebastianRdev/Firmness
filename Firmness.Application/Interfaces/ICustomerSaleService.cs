namespace Firmness.Application.Interfaces;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Sales;

public interface ICustomerSaleService
{
    /// <summary>
    /// Creates a new sale and generates a receipt for the customer.
    /// </summary>
    /// <param name="createDto">The data transfer object containing sale details.</param>
    /// <returns>A result containing the sale response DTO.</returns>
    Task<ResultOft<SaleResponseDto>> CreateSaleWithReceiptAsync(CreateSaleDto createDto);

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <returns>A result containing the sale response DTO if found.</returns>
    Task<ResultOft<SaleResponseDto>> GetSaleByIdAsync(int id);
}
