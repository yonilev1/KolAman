using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationGate.KafkaProducer;
using NotificationGate.ReadAlerts;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Elasticsearch;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "C#-procude-logs-index"
    })
    .WriteTo.File("../logs/logs.log",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Level:u3} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();



IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var config = context.Configuration;
        var ElasticCon = config["Elastic:ConnectionString"]!;
        //var RabbitCon = config["Rabbit:ConnectionString"]!;
        //var SqlCon = config["ConnectionString:DefaultConnect"]!;
        //var bootstrap = config["Kafka:BootstrapServer"]!;

        services.AddSingleton<IElasticsearchClientSettings>
        (new ElasticsearchClientSettings(new Uri(ElasticCon)).DisableDirectStreaming());

        services.AddSingleton<ElasticsearchClient>();

        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        services.AddHostedService<AlertReader>();

    }).Build();

await host.RunAsync();
