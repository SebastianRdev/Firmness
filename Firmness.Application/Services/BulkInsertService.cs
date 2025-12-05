using Firmness.Application.DTOs.Excel;
using Firmness.Application.Interfaces;
using Firmness.Domain.Interfaces;
using Firmness.Domain.Entities;

namespace Firmness.Application.Services;

public class BulkInsertService : IBulkInsertService
{
    private readonly IGenericRepository<Customer> _customerRepo;
    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<Category> _categoryRepo;

    public BulkInsertService(
        IGenericRepository<Customer> customerRepo,
        IGenericRepository<Product> productRepo,
        IGenericRepository<Category> categoryRepo)
    {
        _customerRepo = customerRepo;
        _productRepo = productRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<BulkInsertResultDto> InsertAsync(
        string entityType,
        List<RowValidationResultDto> validRows)
    {
        var result = new BulkInsertResultDto();

        foreach (var row in validRows)
        {
            try
            {
                switch (entityType.ToLower())
                {
                    case "customer":
                        var customer = new Customer
                        {
                            UserName  = row.RowData["Username"],
                            FullName  = row.RowData["FullName"],
                            Address   = row.RowData["Address"],
                            PhoneNumber     = row.RowData["Phone"],
                            Email     = row.RowData["Email"],
                            CreatedAt = DateTime.UtcNow
                        };

                        await _customerRepo.AddAsync(customer);
                        break;

                    case "product":
                        var product = new Product
                        {
                            Name        = row.RowData["Name"],
                            CategoryId = int.Parse(row.RowData["CategoryId"]),
                            Price       = decimal.Parse(row.RowData["Price"]),
                            Stock       = int.Parse(row.RowData["Stock"] ?? "0"),
                            Description = row.RowData["Description"]
                        };

                        await _productRepo.AddAsync(product);
                        break;

                    case "category":
                        var category = new Category
                        {
                            Name = row.RowData["Name"],
                        };

                        await _categoryRepo.AddAsync(category);
                        break;

                    default:
                        throw new Exception($"Unknown entity type: {entityType}");
                }

                result.InsertedCount++;
            }
            catch
            {
                result.FailedRows.Add(row.RowNumber);
            }
        }

        return result;
    }
}
