namespace Firmness.Application.Interfaces;

using Firmness.Domain.Entities;

public interface ISaleService
{
    Task CreateSaleAsync(Sale sale);
    Task<Sale> GetSaleByIdAsync(int id);
}