using CommandDb.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TaskManager.TaskManagerService;

public class ManageTasksAsync : BackgroundService
{
    private readonly ILogger<ManageTasksAsync> _logger;
    //private readonly TaskManegerDbContext _context;
    private readonly IServiceScopeFactory _scopeFactory;

    public ManageTasksAsync(ILogger<ManageTasksAsync> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //while(!stoppingToken.IsCancellationRequested)
        //{
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var sqlHandler = scope.ServiceProvider.GetRequiredService<ISqlHandler>();
                    while (true)
                    {
                        await sqlHandler.Execute();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while processing alerts: {ex}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while processing alerts: {ex}");
        }
    }
}

        //var northMessage = await _context.NorthAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        //if(northMessage != null)
        //{
        //    AlertModel northalert = new AlertModel
        //    {
        //        AlertId = northMessage.AlertId,
        //        Classification = northMessage.Classification,
        //        Content = northMessage.Content,
        //        Lat = northMessage.Lat,
        //        Lon = northMessage.Lon,
        //        Priority = northMessage.Priority,
        //        Source = northMessage.Source,
        //        Status = northMessage.Status,
        //        Timestamp = northMessage.Timestamp,
        //        Title = northMessage.Title
        //    };
        //    AlertModel workerAlert = await ProcessMessage(northalert, "North");
        //    AlertModelNorth workerAlertNorth = new AlertModelNorth
        //    {
        //        AlertId = workerAlert.AlertId,
        //        Classification = workerAlert.Classification,
        //        Content = workerAlert.Content,
        //        Lat = workerAlert.Lat,
        //        Lon = workerAlert.Lon,
        //        Priority = workerAlert.Priority,
        //        Source = workerAlert.Source,
        //        Status = workerAlert.Status,
        //        Timestamp = workerAlert.Timestamp,
        //        Title = workerAlert.Title
        //    };
        //    _context.NorthAlerts.Update(workerAlertNorth);
        //    await _context.SaveChangesAsync();
        //}

        //var centerMessage = await _context.CenterAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        //if (centerMessage != null)
        //{
        //    AlertModel centeralert = new AlertModel
        //    {
        //        AlertId = centerMessage.AlertId,
        //        Classification = centerMessage.Classification,
        //        Content = centerMessage.Content,
        //        Lat = centerMessage.Lat,
        //        Lon = centerMessage.Lon,
        //        Priority = centerMessage.Priority,
        //        Source = centerMessage.Source,
        //        Status = centerMessage.Status,
        //        Timestamp = centerMessage.Timestamp,
        //        Title = centerMessage.Title
        //    };
        //    AlertModel workerAlert = await ProcessMessage(centeralert, "Center");
        //    AlertModelCenter workerAlertCenter = new AlertModelCenter
        //    {
        //        AlertId = workerAlert.AlertId,
        //        Classification = workerAlert.Classification,
        //        Content = workerAlert.Content,
        //        Lat = workerAlert.Lat,
        //        Lon = workerAlert.Lon,
        //        Priority = workerAlert.Priority,
        //        Source = workerAlert.Source,
        //        Status = workerAlert.Status,
        //        Timestamp = workerAlert.Timestamp,
        //        Title = workerAlert.Title
        //    };
        //    _context.CenterAlerts.Update(workerAlertCenter);
        // await _context.SaveChangesAsync();
        //}

        //var southMessage = await _context.SouthAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        //if (southMessage != null)
        //{
        //    AlertModel southalert = new AlertModel
        //    {
        //        AlertId = southMessage.AlertId,
        //        Classification = southMessage.Classification,
        //        Content = southMessage.Content,
        //        Lat = southMessage.Lat,
        //        Lon = southMessage.Lon,
        //        Priority = southMessage.Priority,
        //        Source = southMessage.Source,
        //        Status = southMessage.Status,
        //        Timestamp = southMessage.Timestamp,
        //        Title = southMessage.Title
        //    };
        //    AlertModel workerAlert = await ProcessMessage(southalert, "South");
        //    AlertModelSouth workerAlertSouth = new AlertModelSouth
        //    {
        //        AlertId = workerAlert.AlertId,
        //        Classification = workerAlert.Classification,
        //        Content = workerAlert.Content,
        //        Lat = workerAlert.Lat,
        //        Lon = workerAlert.Lon,
        //        Priority = workerAlert.Priority,
        //        Source = workerAlert.Source,
        //        Status = workerAlert.Status,
        //        Timestamp = workerAlert.Timestamp,
        //        Title = workerAlert.Title
        //    };
        //    _context.SouthAlerts.Update(workerAlertSouth);
        //    await _context.SaveChangesAsync();
        //}

        //var overseasmessage = await _context.OverseasAlerts.FirstOrDefaultAsync(s => s.Status == "WAITING");
        //if (overseasmessage != null)
        //{
        //    AlertModel overseasAlert = new AlertModel
        //    {
        //        AlertId = overseasmessage.AlertId,
        //        Classification = overseasmessage.Classification,
        //        Content = overseasmessage.Content,
        //        Lat = overseasmessage.Lat,
        //        Lon = overseasmessage.Lon,
        //        Priority = overseasmessage.Priority,
        //        Source = overseasmessage.Source,
        //        Status = overseasmessage.Status,
        //        Timestamp = overseasmessage.Timestamp,
        //        Title = overseasmessage.Title
        //    };
        //    AlertModel workerAlert = await ProcessMessage(overseasAlert, "Overseas");
        //    AlertModelOverseas workerAlertOverseas = new AlertModelOverseas
        //    {
        //        AlertId = workerAlert.AlertId,
        //        Classification = workerAlert.Classification,
        //        Content = workerAlert.Content,
        //        Lat = workerAlert.Lat,
        //        Lon = workerAlert.Lon,
        //        Priority = workerAlert.Priority,
        //        Source = workerAlert.Source,
        //        Status = workerAlert.Status,
        //        Timestamp = workerAlert.Timestamp,
        //        Title = workerAlert.Title
        //    };
        //    _context.OverseasAlerts.Update(workerAlertOverseas);
        //    await _context.SaveChangesAsync();
        //}
        //}

    //}
    //}

    //private async Task<AlertModel> ProcessMessage(AlertModel alert, string command)
    //{
    //    if (alert.Priority == "LOW" || (alert.Priority == "MEDIUM" && alert.Classification == "UNCLASSIFIED"))
    //    {
    //        alert.Status = "CANCEL";
    //        return alert;
    //    }
    //    else
    //    {
    //        alert.Status = "INPROGRESS";
    //        AlertModel worketAlert = await WorkTheAlert(alert, command);
    //        return worketAlert;
    //    }
        
    //}

    //private async Task<AlertModel> WorkTheAlert(AlertModel alert, string command)
    //{
    //    if (alert.Source == "Mossad" && (alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
    //        (alert.Classification == "UNCLASSIFIED" || alert.Classification == "RESTRICTED"))
    //    {
    //        await Task.Delay(TimeSpan.FromSeconds(55));
    //        alert.Status = "DONE";
    //        return alert;
    //    }

    //    if ((alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
    //        (alert.Classification == "UNCLASSIFIED" || alert.Classification == "RESTRICTED"))
    //    {
    //        await Task.Delay(TimeSpan.FromSeconds(12));
    //        alert.Status = "DONE";
    //        return alert;
    //    }

    //    if ((alert.Priority == "HIGH" || alert.Priority == "CRITICAL") &&
    //        (alert.Classification == "SECRET" || alert.Classification == "TOP_SECRET"))
    //    {
    //        await Task.Delay(TimeSpan.FromSeconds(35));
    //        alert.Status = "DONE";
    //        return alert;
    //    }

    //    await Task.Delay(TimeSpan.FromSeconds(8));
    //    alert.Status = "DONE";
    //    return alert;
    //}
//}
