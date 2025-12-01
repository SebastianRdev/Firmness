namespace Firmness.Infrastructure.Services;

using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

public class FilePathProvider : IFilePathProvider
{
    private readonly IWebHostEnvironment _environment;

    public FilePathProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string GetReceiptPath(string fileName)
    {
        return Path.Combine(_environment.WebRootPath, "receipts", fileName);
    }
}
