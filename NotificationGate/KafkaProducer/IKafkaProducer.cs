using Confluent.Kafka;
using NotificationGate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.KafkaProducer;

public interface IKafkaProducer
{

    Task<DeliveryResult<Null, string>> Produce(AlertModel alert);
}
