using CommandDb.Handlers;
using CommandDb.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandDb.RabbitConsumer;

public class RabbitConsumerAsync : BackgroundService
{
    private readonly ILogger<RabbitConsumerAsync> _logger;
    private readonly IConnectionFactory _factory;
    protected IConnection? _connection;
    //protected readonly ISqlHandler _sqlHandler;
    private readonly IServiceScopeFactory _scopeFactory;


    public RabbitConsumerAsync(ILogger<RabbitConsumerAsync> logger, IConnectionFactory factory, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _factory = factory;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_connection == null || _connection.IsOpen == false)
        {
            _connection = await _factory.CreateConnectionAsync();
        }
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var channel = await _connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: "direct_alerts", type: ExchangeType.Direct);

        await channel.QueueDeclareAsync(queue: "north", durable: true, autoDelete: false, exclusive: false);
        await channel.QueueDeclareAsync(queue: "south", durable: true, autoDelete: false, exclusive: false);
        await channel.QueueDeclareAsync(queue: "center", durable: true, autoDelete: false, exclusive: false);
        await channel.QueueDeclareAsync(queue: "overseas", durable: true, autoDelete: false, exclusive: false);

        await channel.QueueBindAsync(exchange: "direct_alerts", queue: "north", routingKey: "NORTH");
        await channel.QueueBindAsync(exchange: "direct_alerts", queue: "south", routingKey: "SOUTH");
        await channel.QueueBindAsync(exchange: "direct_alerts", queue: "center", routingKey: "CENTER");
        await channel.QueueBindAsync(exchange: "direct_alerts", queue: "overseas", routingKey: "OVERSEAS");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var alert = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            var jsonAlert = JsonSerializer.Deserialize<AlertModel>(alert)!;
            _logger.LogInformation($" [x] Received '{routingKey}':'{jsonAlert}'");

            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var sqlHandler = scope.ServiceProvider.GetRequiredService<ISqlHandler>();
                    var response = await sqlHandler.Execute(jsonAlert, routingKey);

                    if (response == false)
                    {
                        _logger.LogError($"Error while adding to {routingKey} Database");
                        await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                    else if (response == null || response == true)
                    {
                        await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }

                }
                catch (Exception  ex)
                {
                    Console.WriteLine(ex);
                }
                
               
            }
        };

        await channel.BasicConsumeAsync("north", autoAck: false, consumer: consumer);
        await channel.BasicConsumeAsync("south", autoAck: false, consumer: consumer);
        await channel.BasicConsumeAsync("center", autoAck: false, consumer: consumer);
        await channel.BasicConsumeAsync("overseas", autoAck: false, consumer: consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
