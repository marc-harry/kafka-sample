using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;
using Udemy;

namespace KafkaClient
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Consumer starting up!");

            var config = new GeneralConfiguration()
                .SetConsumerConfig(c =>
                {
                    c.BootstrapServers = "localhost:9092";
                    c.StatisticsIntervalMs = 60000;
                    c.SessionTimeoutMs = 6000;
                    c.EnableAutoOffsetStore = false;
                    c.AutoCommitIntervalMs = 60000; // Every 1 minute
                    c.QueuedMinMessages = 1000000;
                })
                .SetProducerConfig(c => c.BootstrapServers = "localhost:9092")
                .SetSchemaRegistryServer("localhost:8081");
            
            var consumer = new ReviewConsumer(config);
            const int nMessages = 1000000;
            
            var startTime = DateTime.UtcNow.Ticks;
            
            var cts = new CancellationTokenSource();
            consumer.Consume(cts, nMessages);
            
            var duration = DateTime.UtcNow.Ticks - startTime;

            Console.WriteLine($"Consumed {nMessages} messages in {duration/10000.0:F0}ms");
            Console.WriteLine($"{(nMessages) / (duration/10000.0):F0}k msg/s");
        }
    }
}
