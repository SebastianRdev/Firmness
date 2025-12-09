using Firmness.Application.DTOs.Agent;
using Firmness.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Firmness.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly ISqlAgentService _agentService;

    public AgentController(ISqlAgentService agentService)
    {
        _agentService = agentService;
    }

    [HttpPost("query")]
    [Authorize(Roles = "Admin,SuperAdmin")] 
    public async Task<IActionResult> Query([FromBody] AgentQueryDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message is required.");

        var response = await _agentService.ProcessUserQueryAsync(request.Message);
        return Ok(new { response });
    }
}
