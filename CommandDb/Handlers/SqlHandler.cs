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
    private readonly Logger<SqlHandler> _logger;
    private readonly CommandDbContext _context;

    public SqlHandler(Logger<SqlHandler> logger, CommandDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<bool> Execute(AlertModel alert, string Command)
    {
        if(Command == "North")
        {
            await _context.NorthAlerts.AddAsync(alert);
        }
        if (Command == "Center")
        {
            await _context.CenterAlerts.AddAsync(alert);
        }
        if (Command == "South")
        {
            await _context.SouthAlerts.AddAsync(alert);
        }
        if (Command == "OverSeas")
        {
            await _context.OverseasAlerts.AddAsync(alert);
        }
        var added = await _context.SaveChangesAsync();
        if (added > 0)
            return true;
        return false;
    }
}
