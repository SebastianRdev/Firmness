namespace Firmness.Application.Common;

/// <summary>
/// Representa el resultado de una operación que puede fallar.
/// Contiene el dato si fue exitosa, o el mensaje de error si falló.
/// </summary>
/// <typeparam name="T">Tipo del dato de respuesta</typeparam>
public class ResultOft<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;

    // Constructor privado - usamos métodos estáticos para crear instancias
    private ResultOft(bool isSuccess, T? data, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Crea un resultado exitoso con datos.
    /// </summary>
    public static ResultOft<T> Success(T data)
    {
        return new ResultOft<T>(true, data, string.Empty);
    }

    /// <summary>
    /// Crea un resultado fallido con mensaje de error.
    /// </summary>
    public static ResultOft<T> Failure(string errorMessage)
    {
        return new ResultOft<T>(false, default, errorMessage);
    }
}