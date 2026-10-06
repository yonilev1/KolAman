using AlertsApi.Model;
using AlertsApi.Service;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace AlertsApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly ISqlService _service;

    public AlertsController(ISqlService service)
    {
        _service = service;
    }

    [HttpGet("/count_per_command")]
    public async Task<ActionResult<CountAlertsDto>> CountAlertsPerCommand()
    {
        return Ok(await _service.CountAlertsPerCommand());
    }

    [HttpGet("count_per_command_per_priority")]
    public async Task<ActionResult<CountAlertsPerCommandBypriority>> CountAlertsPerCommandBypriority()
    {
        return Ok(await _service.CountAlertsPerCommandBypriority());
    }

    [HttpGet("count_per_command_per_status")]
    public async Task<ActionResult<CountAlertsPerCommandByStatus>> CountAlertsPerCommandByStatus()
    {
        return Ok(await _service.CountAlertsPerCommandByStatus());
    }

    [HttpGet("get-hotest-command")]
    public async Task<ActionResult<HotestCommand>> GetHotestCommand()
    {
        return Ok(await _service.GetHotestCommand());
    }
}
