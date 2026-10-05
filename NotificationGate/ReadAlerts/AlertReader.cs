using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationGate.KafkaProducer;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using NotificationGate.Model;
using System.Text.Json;
namespace NotificationGate.ReadAlerts;

class AlertReader : BackgroundService
{
    private readonly ILogger<AlertReader> _logger;
    private readonly IKafkaProducer _producer;

    public AlertReader(ILogger<AlertReader> logger, IKafkaProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        using var watcher = new FileSystemWatcher($@"{currentDirectory}\alert-simulator");
        watcher.InternalBufferSize = 65536;


        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        watcher.Created += OnCreated;

        watcher.Filter = "*.ready";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            var directoryPath = e.FullPath.Replace("alert.ready", "");
            File.Delete(e.FullPath);

            string[] message = Directory.GetFiles(directoryPath);
            var alert = File.ReadAllText(message[0]);
            //Console.WriteLine(alert);
            var alertJson = JsonSerializer.Deserialize<AlertModel>(alert)!;

            _logger.LogInformation($"Read alert: {alertJson.AlertId}, sending to kafka.");

            _producer.Produce(alertJson);
            File.Delete(message[0]);
            Directory.Delete(directoryPath);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Message: {ex.Message}");
        }
        
    }
}