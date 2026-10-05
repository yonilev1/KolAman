using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationGate.KafkaProducer;
using NotificationGate.ReadAlerts;
using NotificationGate.Model;
using Serilog;
using Serilog.Sinks.Elasticsearch;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "Csharp-procude-logs-index"
    })
    .WriteTo.File("../logs/logs.log",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Level:u3} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();



IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        services.AddHostedService<AlertReader>();

    }).Build();

 await host.RunAsync();
