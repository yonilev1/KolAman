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

    public async Task<CountAlertsPerCommandByStatus> CountAlertsPerCommandByStatus()
    {
        return new CountAlertsPerCommandByStatus
        {
            North = new Status
            {
                Waiting = _context.NorthAlerts.Count(c => c.Status == "WAITING"),
                Canceled = _context.NorthAlerts.Count(c => c.Status == "CANCELED"),
                InPrograss = _context.NorthAlerts.Count(c => c.Status == "INPROGRESS"),
                Done = _context.NorthAlerts.Count(c => c.Status == "DONE")
            },
            South = new Status
            {
                Waiting = _context.SouthAlerts.Count(c => c.Status == "WAITING"),
                Canceled = _context.SouthAlerts.Count(c => c.Status == "CANCELED"),
                InPrograss = _context.SouthAlerts.Count(c => c.Status == "INPROGRESS"),
                Done = _context.SouthAlerts.Count(c => c.Status == "DONE")
            },
            Center = new Status
            {
                Waiting = _context.CenterAlerts.Count(c => c.Status == "WAITING"),
                Canceled = _context.CenterAlerts.Count(c => c.Status == "CANCELED"),
                InPrograss = _context.CenterAlerts.Count(c => c.Status == "INPROGRESS"),
                Done = _context.CenterAlerts.Count(c => c.Status == "DONE")
            },
            Overseas = new Status
            {
                Waiting = _context.OverseasAlerts.Count(c => c.Status == "WAITING"),
                Canceled = _context.OverseasAlerts.Count(c => c.Status == "CANCELED"),
                InPrograss = _context.OverseasAlerts.Count(c => c.Status == "INPROGRESS"),
                Done = _context.OverseasAlerts.Count(c => c.Status == "DONE")
            }
        };
    }

    public async Task<HotestCommand> GetHotestCommand()
    {
        var northCritical = _context.NorthAlerts.Count(c => c.Priority == "CRITICAL");
        var northHigh = _context.NorthAlerts.Count(c => c.Priority == "HIGH");
        var northAmount = _context.NorthAlerts.Count();

        var southCritical = _context.NorthAlerts.Count(c => c.Priority == "CRITICAL");
        var southHigh = _context.NorthAlerts.Count(c => c.Priority == "HIGH");
        var southAmount = _context.NorthAlerts.Count();

        var centerCritical = _context.NorthAlerts.Count(c => c.Priority == "CRITICAL");
        var centerHigh = _context.NorthAlerts.Count(c => c.Priority == "HIGH");
        var centerAmount = _context.NorthAlerts.Count();

        var overCritical = _context.NorthAlerts.Count(c => c.Priority == "CRITICAL");
        var overHigh = _context.NorthAlerts.Count(c => c.Priority == "HIGH");
        var overAmount = _context.NorthAlerts.Count();

        int north = 0;
        int south = 0;
        int center = 0;
        int over = 0;

        if (northCritical > southCritical && northCritical > centerCritical && northCritical > overCritical)
            north++;
        if (northHigh > southHigh && northHigh > centerHigh && northHigh > overHigh)
            north++;
        if (northAmount > southAmount && northAmount > centerAmount && northAmount > overAmount)
            north++;

        if (southCritical > northCritical && southCritical > centerCritical && southCritical > overCritical)
            south++;
        if (southHigh > northHigh && southHigh > centerHigh && southHigh > overHigh)
            south++;
        if (southAmount > northAmount && southAmount > northAmount && southAmount > overAmount)
            south++;

        if (centerCritical > southCritical && centerCritical > northCritical && centerCritical > overCritical)
            center++;
        if (centerHigh > southHigh && centerHigh > northHigh && centerHigh > overHigh)
            center++;
        if (centerAmount > southAmount && centerAmount > northAmount && centerAmount > overAmount)
            center++;


        if (northCritical > southCritical && northCritical > centerCritical && northCritical > overCritical)
            north++;
        if (northHigh > southHigh && northHigh > centerHigh && northHigh > overHigh)
            north++;
        if (northAmount > southAmount && northAmount > centerAmount && northAmount > centerAmount)
            north++;

        if(north > south && north > center && north > over)
        {
            return new HotestCommand
            {
                Command = "North",
                Critial = northCritical,
                High = northHigh,
                Amount = northAmount
            };
        }
        else if(south > north && south > center && south > over)
        {
            return new HotestCommand
            {
                Command = "South",
                Critial = southCritical,
                High = southHigh,
                Amount = southAmount
            };
        }
        else if (center > north && center > south && center > over)
        {
            return new HotestCommand
            {
                Command = "Center",
                Critial = centerCritical,
                High = centerHigh,
                Amount = centerAmount
            };
        }
        return new HotestCommand
        {
            Command = "Overseas",
            Critial = overCritical,
            High = overHigh,
            Amount = overAmount
        };
    }

    public async Task<CountAlertsDto> CountAlertsPerCommandPerTitle(string title)
    {
        return new CountAlertsDto
        {
            North = _context.NorthAlerts.Count(c => c.Title == title),
            South = _context.SouthAlerts.Count(c => c.Title == title),
            Center = _context.CenterAlerts.Count(c => c.Title == title),
            Overseas = _context.OverseasAlerts.Count(c => c.Title == title)
        };
    }
}

