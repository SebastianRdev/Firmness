namespace Firmness.Application.Common;

using System.Text.Json.Serialization; // <--- 1. IMPORTANTE: Agrega esto

/// <summary>
/// It represents the result of an operation that may fail.
/// It contains the data if it was successful, or the error message if it failed.
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class ResultOft<T>
{
    // Al usar [JsonConstructor], el deserializador sabrá mapear los parámetros a estas propiedades
    // aunque tengan 'private set'.
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    // 2. IMPORTANTE: Agrega el atributo [JsonConstructor] y cambia a 'public'.
    // Esto le dice al sistema: "Usa este constructor cuando conviertas el JSON a objeto".
    [JsonConstructor]
    public ResultOft(bool isSuccess, T? data, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage ?? string.Empty;
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