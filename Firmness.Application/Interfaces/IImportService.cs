using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firmness.Application.Interfaces;

public interface IImportService
{
    Task<bool> ImportAsync(string entityType, List<Dictionary<string, string>> rows);
}
