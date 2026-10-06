using TaskManager.Data;
using Elastic.Clients.Elasticsearch.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TaskManager.TaskManagerService;
using CommandDb.Handlers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "Csharp-task-manager-logs-index"
    })
    .WriteTo.File("../logs/task-manager-logs.log",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} - {Level:u3} - {Message:lj}{NewLine}{Exception}")
    .CreateLogger();



IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TaskManegerDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<ISqlHandler, SqlHandler>();

        services.AddHostedService<ManageTasksAsync>();

    }).Build();

using (var scope = host.Services.CreateScope())
{
    var handler = scope.ServiceProvider.GetRequiredService<TaskManegerDbContext>();
    handler.Database.EnsureCreated();
    Console.WriteLine("created db");
}

await host.RunAsync();
