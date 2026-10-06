using CommandDb.Data;
using CommandDb.Handlers;
using CommandDb.RabbitConsumer;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Elasticsearch;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "Csharp-rabbit-consumer-logs-index"
    })
    .WriteTo.File("../logs/rabbit-consumer-logs.log",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Level:u3} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();



IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var rabbitCon = configuration["Rabbit:ConnectionString"]!;

        services.AddDbContext<CommandDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddSingleton<IConnectionFactory>(sp =>
        new ConnectionFactory
        {
            Uri = new Uri(rabbitCon)
        });

        services.AddScoped<ISqlHandler ,SqlHandler>();

        services.AddHostedService<RabbitConsumerAsync>();

    }).Build();

using(var scope = host.Services.CreateScope())
{
    var handler = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
    handler.Database.EnsureCreated();
    Console.WriteLine("created db");
}

await host.RunAsync();
