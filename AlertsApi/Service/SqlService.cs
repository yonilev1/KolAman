using AlertsApi.Data;
using AlertsApi.Model;

namespace AlertsApi.Service;

public class SqlService : ISqlService
{
    private readonly ILogger<SqlService> _logger;
    private readonly CommandDbContext _context;

    public SqlService(ILogger<SqlService> logger, CommandDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<CountAlertsDto> CountAlertsPerCommand()
    {
        return new CountAlertsDto
        {
            North = _context.NorthAlerts.Count(),
            South = _context.SouthAlerts.Count(),
            Center = _context.CenterAlerts.Count(),
            Overseas = _context.OverseasAlerts.Count()
        };
    }

    public async Task<CountAlertsPerCommandBypriority> CountAlertsPerCommandBypriority()
    {
        return new CountAlertsPerCommandBypriority
        {
            North = new Priority
            {
                Low = _context.NorthAlerts.Count(c => c.Priority == "LOW"),
                Medium = _context.NorthAlerts.Count(c => c.Priority == "MEDIUM"),
                High = _context.NorthAlerts.Count(c => c.Priority == "HIGH"),
                Critial = _context.NorthAlerts.Count(c => c.Priority == "CRITICAL")
            },
            South = new Priority
            {
                Low = _context.SouthAlerts.Count(c => c.Priority == "LOW"),
                Medium = _context.SouthAlerts.Count(c => c.Priority == "MEDIUM"),
                High = _context.SouthAlerts.Count(c => c.Priority == "HIGH"),
                Critial = _context.SouthAlerts.Count(c => c.Priority == "CRITICAL")
            },
            Center = new Priority
            {
                Low = _context.CenterAlerts.Count(c => c.Priority == "LOW"),
                Medium = _context.CenterAlerts.Count(c => c.Priority == "MEDIUM"),
                High = _context.CenterAlerts.Count(c => c.Priority == "HIGH"),
                Critial = _context.CenterAlerts.Count(c => c.Priority == "CRITICAL")
            },
            Overseas = new Priority
            {
                Low = _context.OverseasAlerts.Count(c => c.Priority == "LOW"),
                Medium = _context.OverseasAlerts.Count(c => c.Priority == "MEDIUM"),
                High = _context.OverseasAlerts.Count(c => c.Priority == "HIGH"),
                Critial = _context.OverseasAlerts.Count(c => c.Priority == "CRITICAL")
            }
        };
    }

}
