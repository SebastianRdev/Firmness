using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.WebAdmin.Controllers;

[Authorize]
public class AgentController : Controller
{
    private readonly ISqlAgentService _sqlAgentService;

    public AgentController(ISqlAgentService sqlAgentService)
    {
        _sqlAgentService = sqlAgentService;
    }

    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { response = "Message is required" });

        var response = await _sqlAgentService.ProcessUserQueryAsync(request.Message);
        return Ok(new { response });
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
