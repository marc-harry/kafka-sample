using System;
using System.Threading;
using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;

namespace Kafka.Common.Infrastructure
{
    public abstract class BaseConsumer<TKey, TValue> : IBaseConsumer<TKey, TValue> where TValue : ISpecificRecord
    {
        private readonly string _groupId;
        private readonly string _topicName;

        protected BaseConsumer(string groupId, string topicName)
        {
            _groupId = groupId;
            _topicName = topicName;
        }
        
        public async Task ConsumeAsync()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = GeneralConfiguration.BootstrapServer,
                GroupId = _groupId,
                EnableAutoCommit = false,
                StatisticsIntervalMs = 60000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(SchemaConfiguration.SchemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_topicName);
                
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
                            await HandleMessage(cr);
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

        protected abstract Task HandleMessage(ConsumeResult<TKey, TValue> result);
    }
}