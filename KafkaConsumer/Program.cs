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

            var config = new GeneralConfiguration
            {
                ConsumerConfig = new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    StatisticsIntervalMs = 60000,
                    SessionTimeoutMs = 6000
                },
                ProducerConfig = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092"
                }
            };
            config.SetSchemaRegistryServer("localhost:8081");
            
            var consumer = new ReviewConsumer(config);
            var nMessages = 1000000;
            
            var startTime = DateTime.UtcNow.Ticks;
            
            var cts = new CancellationTokenSource();
            consumer.Consume(cts, nMessages);
            
            var duration = DateTime.UtcNow.Ticks - startTime;

            Console.WriteLine($"Consumed {nMessages-1} messages in {duration/10000.0:F0}ms");
            Console.WriteLine($"{(nMessages-1) / (duration/10000.0):F0}k msg/s");
        }
    }
}
