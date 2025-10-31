namespace Firmness.WebAdmin.Services;

using Firmness.Core.Entities;
using Firmness.Core.Interfaces;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddProduct(Product product)
    {
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.CompleteAsync(); // Guardamos los cambios
    }
}