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
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Consumer starting up!");

            var consumer = new ReviewConsumer("review-consumer");

            await consumer.ConsumeAsync();
        }
    }
}
