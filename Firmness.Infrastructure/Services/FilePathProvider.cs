namespace Firmness.Infrastructure.Services;

using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

/// <summary>
/// Provides file paths for various resources.
/// </summary>
public class FilePathProvider : IFilePathProvider
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePathProvider"/> class.
    /// </summary>
    /// <param name="environment">The web host environment.</param>
    public FilePathProvider(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Gets the full file path for a receipt.
    /// </summary>
    /// <param name="fileName">The name of the receipt file.</param>
    /// <returns>The full path to the receipt file.</returns>
    public string GetReceiptPath(string fileName)
    {
        return Path.Combine(_environment.WebRootPath, "receipts", fileName);
    }
}
