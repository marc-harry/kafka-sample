using Confluent.Kafka;
using System;
using System.Threading;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;
using Udemy;

namespace KafkaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer starting up!");

            var config = new ConsumerConfig
            {
                BootstrapServers = GeneralConfiguration.BootstrapServer,
                GroupId = "review-processor",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 60000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(SchemaConfiguration.SchemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<long, Review>(config)
                .SetValueDeserializer(new AvroDeserializer<Review>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(TopicNames.NewReviews.GetDescription());
                
                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };

                var count = 0;
                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cts.Token);
                            Console.WriteLine($"Consumed message '{cr.Value.Title}' at: '{cr.TopicPartitionOffset}'.");
                            count++;

                            // Commit every 10 reviews
                            if (count != 10) continue;
                            consumer.Commit(cr);
                            count = 0;
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }
    }
}
