using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationGate.ReadAlerts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.KafkaProducer;

public class KafkaProducer : IKafkaProducer
{
    private readonly ILogger<KafkaProducer> _logger;
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(ILogger<KafkaProducer> logger, IProducer<Null, string>producer, IConfiguration configuration)
    {
        _logger = logger;

        var bootstrap = configuration["Kafka:BootstrapServer"] ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrap,
            ClientId = "c1"
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task<bool> Produce(string alert)
    {
        return true;
    }
}
