namespace Firmness.Application.Interfaces;

using Firmness.Application.Common;
using Firmness.Application.DTOs.Sales;

public interface ICustomerSaleService
{
    Task<ResultOft<SaleResponseDto>> CreateSaleWithReceiptAsync(CreateSaleDto createDto);
    Task<ResultOft<SaleResponseDto>> GetSaleByIdAsync(int id);
}
