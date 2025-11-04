namespace Firmness.Application.Common;

/// <summary>
/// It represents the result of an operation that may fail.
/// It contains the data if it was successful, or the error message if it failed.
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class ResultOft<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    // Private constructor - we use static methods to create instances
    private ResultOft(bool isSuccess, T? data, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Create a successful outcome with data.
    /// </summary>
    public static ResultOft<T> Success(T data)
    {
        return new ResultOft<T>(true, data, string.Empty);
    }

    /// <summary>
    /// Create a failed result with an error message.
    /// </summary>
    public static ResultOft<T> Failure(string errorMessage)
    {
        return new ResultOft<T>(false, default, errorMessage);
    }
}