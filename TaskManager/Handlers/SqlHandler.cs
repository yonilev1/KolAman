using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Model;

namespace CommandDb.Handlers;

public class SqlHandler : ISqlHandler
{
    private readonly ILogger<SqlHandler> _logger;
    private readonly TaskManegerDbContext _context;

    public SqlHandler(ILogger<SqlHandler> logger, TaskManegerDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Execute()
    {
        var northMessage = await _context.NorthAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        if (northMessage != null)
        {
            AlertModel northalert = new AlertModel
            {
                AlertId = northMessage.AlertId,
                Classification = northMessage.Classification,
                Content = northMessage.Content,
                Lat = northMessage.Lat,
                Lon = northMessage.Lon,
                Priority = northMessage.Priority,
                Source = northMessage.Source,
                Status = northMessage.Status,
                Timestamp = northMessage.Timestamp,
                Title = northMessage.Title
            };
            DateTime start = DateTime.Now;
            AlertModel workerAlert = await ProcessMessage(northalert, "North");
            AlertModelNorth workerAlertNorth = new AlertModelNorth
            {
                AlertId = workerAlert.AlertId,
                Classification = workerAlert.Classification,
                Content = workerAlert.Content,
                Lat = workerAlert.Lat,
                Lon = workerAlert.Lon,
                Priority = workerAlert.Priority,
                Source = workerAlert.Source,
                Status = workerAlert.Status,
                Timestamp = workerAlert.Timestamp,
                Title = workerAlert.Title
            };
            _context.NorthAlerts.Update(workerAlertNorth);
            DateTime End = DateTime.Now;
            _logger.LogTrace($"{workerAlertNorth.AlertId}: {End - start}");
            await _context.SaveChangesAsync();
        }

        var centerMessage = await _context.CenterAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        if (centerMessage != null)
        {
            AlertModel centeralert = new AlertModel
            {
                AlertId = centerMessage.AlertId,
                Classification = centerMessage.Classification,
                Content = centerMessage.Content,
                Lat = centerMessage.Lat,
                Lon = centerMessage.Lon,
                Priority = centerMessage.Priority,
                Source = centerMessage.Source,
                Status = centerMessage.Status,
                Timestamp = centerMessage.Timestamp,
                Title = centerMessage.Title
            };
            DateTime start = DateTime.Now;
            AlertModel workerAlert = await ProcessMessage(centeralert, "Center");
            AlertModelCenter workerAlertCenter = new AlertModelCenter
            {
                AlertId = workerAlert.AlertId,
                Classification = workerAlert.Classification,
                Content = workerAlert.Content,
                Lat = workerAlert.Lat,
                Lon = workerAlert.Lon,
                Priority = workerAlert.Priority,
                Source = workerAlert.Source,
                Status = workerAlert.Status,
                Timestamp = workerAlert.Timestamp,
                Title = workerAlert.Title
            };
            _context.CenterAlerts.Update(workerAlertCenter);
            DateTime End = DateTime.Now;
            _logger.LogTrace($"{workerAlertCenter.AlertId}: {End - start}");
            await _context.SaveChangesAsync();
        }

        var southMessage = await _context.SouthAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        if (southMessage != null)
        {
            AlertModel southalert = new AlertModel
            {
                AlertId = southMessage.AlertId,
                Classification = southMessage.Classification,
                Content = southMessage.Content,
                Lat = southMessage.Lat,
                Lon = southMessage.Lon,
                Priority = southMessage.Priority,
                Source = southMessage.Source,
                Status = southMessage.Status,
                Timestamp = southMessage.Timestamp,
                Title = southMessage.Title
            };
            DateTime start = DateTime.Now;
            AlertModel workerAlert = await ProcessMessage(southalert, "South");
            AlertModelSouth workerAlertSouth = new AlertModelSouth
            {
                AlertId = workerAlert.AlertId,
                Classification = workerAlert.Classification,
                Content = workerAlert.Content,
                Lat = workerAlert.Lat,
                Lon = workerAlert.Lon,
                Priority = workerAlert.Priority,
                Source = workerAlert.Source,
                Status = workerAlert.Status,
                Timestamp = workerAlert.Timestamp,
                Title = workerAlert.Title
            };
            _context.SouthAlerts.Update(workerAlertSouth);
            DateTime End = DateTime.Now;
            _logger.LogTrace($"{workerAlertSouth.AlertId}: {End - start}");
            await _context.SaveChangesAsync();
        }

        var overseasmessage = await _context.OverseasAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        if (overseasmessage != null)
        {
            AlertModel overseasAlert = new AlertModel
            {
                AlertId = overseasmessage.AlertId,
                Classification = overseasmessage.Classification,
                Content = overseasmessage.Content,
                Lat = overseasmessage.Lat,
                Lon = overseasmessage.Lon,
                Priority = overseasmessage.Priority,
                Source = overseasmessage.Source,
                Status = overseasmessage.Status,
                Timestamp = overseasmessage.Timestamp,
                Title = overseasmessage.Title
            };
            DateTime start = DateTime.Now;
            AlertModel workerAlert = await ProcessMessage(overseasAlert, "Overseas");
            AlertModelOverseas workerAlertOverseas = new AlertModelOverseas
            {
                AlertId = workerAlert.AlertId,
                Classification = workerAlert.Classification,
                Content = workerAlert.Content,
                Lat = workerAlert.Lat,
                Lon = workerAlert.Lon,
                Priority = workerAlert.Priority,
                Source = workerAlert.Source,
                Status = workerAlert.Status,
                Timestamp = workerAlert.Timestamp,
                Title = workerAlert.Title
            };
            _context.OverseasAlerts.Update(workerAlertOverseas);
            DateTime End = DateTime.Now;
            _logger.LogTrace($"{workerAlertOverseas.AlertId}: {End - start}");
            await _context.SaveChangesAsync();
        }
    }

    public async Task<AlertModel> ProcessMessage(AlertModel alert, string command)
    {
        if (alert.Priority == "LOW" || (alert.Priority == "MEDIUM" && alert.Classification == "UNCLASSIFIED"))
        {
            alert.Status = "CANCEL";
            return alert;
        }
        else
        {
            alert.Status = "INPROGRESS";
            AlertModel worketAlert = await WorkTheAlert(alert, command);
            return worketAlert;
        }

    }

    public async Task<AlertModel> WorkTheAlert(AlertModel alert, string command)
    {
        if (alert.Source == "Mossad" && (alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
            (alert.Classification == "UNCLASSIFIED" || alert.Classification == "RESTRICTED"))
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            alert.Status = "DONE";
            return alert;
        }

        if ((alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
            (alert.Classification == "UNCLASSIFIED" || alert.Classification == "RESTRICTED"))
        {
            await Task.Delay(TimeSpan.FromSeconds(6));
            alert.Status = "DONE";
            return alert;
        }

        if ((alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
            (alert.Classification == "SECRET" || alert.Classification == "TOP_SECRET"))
        {
            await Task.Delay(TimeSpan.FromSeconds(7));
            alert.Status = "DONE";
            return alert;
        }

        await Task.Delay(TimeSpan.FromSeconds(2));
        alert.Status = "DONE";
        return alert;
    }
}


    
