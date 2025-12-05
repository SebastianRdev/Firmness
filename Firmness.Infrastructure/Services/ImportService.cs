using Firmness.Application.Interfaces;
using Firmness.Application.Services;
using Firmness.Application.MappingTemplates;
using Firmness.Domain.Entities;
using Firmness.Domain.Interfaces;

namespace Firmness.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly IGenericRepository<Customer> _customerRepo;

    public ImportService(IGenericRepository<Customer> customerRepo)
    {
        _customerRepo = customerRepo;
    }

    public async Task<bool> ImportAsync(string entityType, List<Dictionary<string, string>> rows)
    {
        var template = ColumnTemplateFactory.GetTemplate(entityType);

        switch (entityType.ToLower())
        {
            case "customer":
                var customers = new List<Customer>();

                foreach (var row in rows)
                {
                    var c = RowToEntityMapper.MapRow<Customer>(row, template);
                    customers.Add(c);
                }

                await _customerRepo.AddRangeAsync(customers);
                await _customerRepo.SaveChangesAsync();
                break;

            default:
                throw new Exception($"Import no implementado para {entityType}");
        }

        return true;
    }
}
