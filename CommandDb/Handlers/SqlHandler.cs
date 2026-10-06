using CommandDb.Data;
using CommandDb.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandDb.Handlers;

public class SqlHandler : ISqlHandler
{
    private readonly ILogger<SqlHandler> _logger;
    private readonly CommandDbContext _context;

    public SqlHandler(ILogger<SqlHandler> logger, CommandDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool?> Execute(AlertModel alert, string Command)
    {
        if(ValidateAlert(alert))
        {
            
            if (Command == "NORTH")
            {
                AlertModelNorth north = new AlertModelNorth
                {
                    AlertId = alert.AlertId,
                    Classification = alert.Classification,
                    Content = alert.Content,
                    Lat = alert.Lat,
                    Lon = alert.Lon,
                    Priority = alert.Priority,
                    Source = alert.Source,
                    Status = alert.Status,
                    Timestamp = alert.Timestamp,
                    Title = alert.Title
                };
                await _context.NorthAlerts.AddAsync(north);
            }
            if (Command == "CENTER")
            {
                AlertModelCenter center = new AlertModelCenter
                {
                    AlertId = alert.AlertId,
                    Classification = alert.Classification,
                    Content = alert.Content,
                    Lat = alert.Lat,
                    Lon = alert.Lon,
                    Priority = alert.Priority,
                    Source = alert.Source,
                    Status = alert.Status,
                    Timestamp = alert.Timestamp,
                    Title = alert.Title
                };
                await _context.CenterAlerts.AddAsync(center);
            }
            if (Command == "SOUTH")
            {
                AlertModelSouth south = new AlertModelSouth
                {
                    AlertId = alert.AlertId,
                    Classification = alert.Classification,
                    Content = alert.Content,
                    Lat = alert.Lat,
                    Lon = alert.Lon,
                    Priority = alert.Priority,
                    Source = alert.Source,
                    Status = alert.Status,
                    Timestamp = alert.Timestamp,
                    Title = alert.Title
                };
                await _context.SouthAlerts.AddAsync(south);
            }
            if (Command == "OVERSEAS")
            {
                AlertModelOverseas overseas = new AlertModelOverseas
                {
                    AlertId = alert.AlertId,
                    Classification = alert.Classification,
                    Content = alert.Content,
                    Lat = alert.Lat,
                    Lon = alert.Lon,
                    Priority = alert.Priority,
                    Source = alert.Source,
                    Status = alert.Status,
                    Timestamp = alert.Timestamp,
                    Title = alert.Title
                };
                await _context.OverseasAlerts.AddAsync(overseas);
            }
            var added = await _context.SaveChangesAsync();
            Console.WriteLine($"added is {added}");
            if (added > 0)
                return true;
            return false;
        }
        return null;
    }

    public bool ValidateAlert(AlertModel alert)
    {
        if (string.IsNullOrEmpty(alert.AlertId) ||
            string.IsNullOrEmpty(alert.Classification) ||
            string.IsNullOrEmpty(alert.Content) ||
            string.IsNullOrEmpty(alert.Priority) ||
            string.IsNullOrEmpty(alert.Source) ||
            string.IsNullOrEmpty(alert.Status) ||
            string.IsNullOrEmpty(alert.Timestamp) ||
            string.IsNullOrEmpty(alert.Title))
        {
            _logger.LogError($"alert is missing some fields.");
            return false;
        }

        if (alert.Source == "aman" &&
        alert.Title != "זוהה כלי טיס בלתי מאויש עוין" &&
        alert.Title != "זוהו הכנות לשיגור" &&
        alert.Title != "זוהה שיגור טיל בליסטי" &&
        alert.Title != "שיבושי ניווט באזור" &&
        alert.Title != "תנועת כוחות חריגה סמוך לגבול" &&
        alert.Title != "זוהה שיגור רקטות")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid source");
            return false;
        }

        if (alert.Source == "mossad" &&
            alert.Title != "התרעה על כוונה לפגוע ביעד ישראלי בחוץ לארץ" &&
            alert.Title != "זוהה נתיב הברחת אמצעי לחימה" &&
            alert.Title != "פעילות חריגה באתר אסטרטגי" &&
            alert.Title != "ניסיון כניסה של פעיל עוין לישראל" &&
            alert.Title != "העברת כספים לארגון טרור" &&
            alert.Title != "תנועת פעיל עוין בין מדינות")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid source");
            return false;
        }

        if (alert.Source == "pikud-haoref" &&
            alert.Title != "ירי רקטות וטילים" &&
            alert.Title != "חדירת כלי טיס עוין" &&
            alert.Title != "חדירת מחבלים" &&
            alert.Title != "התרעה מקדימה" &&
            alert.Title != "רעידת אדמה" &&
            alert.Title != "האירוע הסתיים")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid source");
            return false;
        }

        if (alert.Source == "shabak" &&
            alert.Title != "התרעה חמה לפיגוע" &&
            alert.Title != "תנועת מחבל מבוקש" &&
            alert.Title != "חשד לחדירה ליישוב" &&
            alert.Title != "רכב חשוד" &&
            alert.Title != "חשד לפעילות ריגול עבור גורם עוין" &&
            alert.Title != "גניבת אמצעי לחימה")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid source");
            return false;
        }

        if (alert.Priority != "CRITICAL" &&
            alert.Priority != "HIGH" &&
            alert.Priority != "MEDIUM" &&
            alert.Priority != "LOW")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid priority");
            return false;
        }

        if (alert.Classification != "UNCLASSIFIED" &&
            alert.Classification != "RESTRICTED" &&
            alert.Classification != "SECRET" &&
            alert.Classification != "TOP_SECRET")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid classification");
            return false;
        }

        if (alert.Lat < -90 || alert.Lat > 90)
        {
            _logger.LogError($"alert {alert.AlertId} has out of range lat");
            return false;
        }

        if (alert.Lon < -180 || alert.Lon > 180)
        {
            _logger.LogError($"alert {alert.AlertId} has out of range lon");
            return false;
        }

        if (alert.Status != "WAITING")
        {
            _logger.LogError($"alert {alert.AlertId} has invalid status");
            return false;
        }

        return true;
    }


}
