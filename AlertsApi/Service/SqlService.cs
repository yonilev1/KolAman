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

}
