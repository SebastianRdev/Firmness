using System.Threading.Tasks;

namespace Firmness.Application.Interfaces
{
    public interface ISqlAgentService
    {
        Task<string> ProcessUserQueryAsync(string userMessage);
    }
}
