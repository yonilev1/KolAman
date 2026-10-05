using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationGate.Model;
using NotificationGate.ReadAlerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationGate.KafkaProducer;

public class KafkaProducer : IKafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(ILogger<KafkaProducer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _topic = configuration["Kafka:Topic"] ?? "alerts-topic";
        var bootstrap = configuration["Kafka:BootstrapServer"] ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrap,
            ClientId = "c1"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task<DeliveryResult<Null, string>> Produce(AlertModel alertJson)
    {
        try
        {
            var alert = JsonSerializer.Serialize(alertJson);

            var message = new Message<Null, string>
            {
                Value = alert
            };

            var response = await _producer.ProduceAsync(_topic, message);
            _logger.LogInformation($"sent alert: {alertJson.AlertId} ");
            return response;
        }
        catch(Exception ex)
        {
            _logger.LogInformation($"Error while sending: {alertJson.AlertId} to kafka. {ex}");
            return new DeliveryResult<Null, string>();
        }
    }
}
