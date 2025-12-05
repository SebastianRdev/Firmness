namespace Firmness.Application.Interfaces;

public interface IFilePathProvider
{
    /// <summary>
    /// Gets the full path for storing receipts.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <returns>The full file system path to the file.</returns>
    string GetReceiptPath(string fileName);
}
