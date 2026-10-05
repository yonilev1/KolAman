using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationGate.KafkaProducer;

public interface IKafkaProducer
{

    Task<bool> Produce(string alert);
}
