namespace Firmness.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Firmness.Application.Common;

/// <summary>
/// Base controller with common helper methods for API controllers
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Maps a ResultOft to an appropriate IActionResult
    /// </summary>
    protected IActionResult MapResultToActionResult<T>(ResultOft<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);

        return MapFailure(result);
    }

    /// <summary>
    /// Maps a ResultOft failure to an appropriate HTTP status code
    /// </summary>
    protected IActionResult MapFailure<T>(ResultOft<T> result)
    {
        var error = new { error = result.ErrorMessage };
        
        if (result.ErrorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(error);
        
        if (result.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            return Conflict(error);
        
        return BadRequest(error);
    }

    /// <summary>
    /// Maps a Result failure to an appropriate HTTP status code
    /// </summary>
    protected IActionResult MapFailure(Result result)
    {
        var message = result.ErrorMessage ?? "An error occurred";
        
        if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new { error = message });
        
        return BadRequest(new { error = message });
    }
}
