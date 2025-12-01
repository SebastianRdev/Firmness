namespace Firmness.Application.Interfaces;

public interface IFilePathProvider
{
    /// <summary>
    /// Gets the full path for storing receipts
    /// </summary>
    /// <param name="fileName">The file name</param>
    /// <returns>Full path to the file</returns>
    string GetReceiptPath(string fileName);
}
