namespace Firmness.Infrastructure.Services.Identity;

public class AuthResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static AuthResult SuccessResult() => new AuthResult { Success = true };
    public static AuthResult Failure(string message) => new AuthResult { Success = false, Message = message };
}