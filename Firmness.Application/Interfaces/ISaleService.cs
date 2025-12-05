namespace Firmness.Application.Interfaces;

using Firmness.Domain.Entities;

public interface ISaleService
{
    /// <summary>
    /// Creates a new sale transaction.
    /// </summary>
    /// <param name="sale">The sale entity to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateSaleAsync(Sale sale);

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <returns>The sale entity if found.</returns>
    Task<Sale> GetSaleByIdAsync(int id);
}