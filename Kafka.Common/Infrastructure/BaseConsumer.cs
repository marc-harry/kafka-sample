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
        private readonly GeneralConfiguration _configuration;
        private readonly string _groupId;
        private readonly string _topicName;

        protected BaseConsumer(GeneralConfiguration configuration, string groupId, string topicName)
        {
            _configuration = configuration;
            _groupId = groupId;
            _topicName = topicName;
        }
        
        public async Task ConsumeAsync(CancellationTokenSource cancellationTokenSource)
        {
            var config = _configuration.ConsumerConfig;
            config.GroupId = _groupId;

            using (var schemaRegistry = new CachedSchemaRegistryClient(_configuration.SchemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_topicName);

                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancellationTokenSource.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cancellationTokenSource.Token);
                            await HandleMessageAsync(cr);
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

        public void Consume(CancellationTokenSource cancellationTokenSource, long? messageCount = null)
        {
            var config = _configuration.ConsumerConfig;
            config.GroupId = _groupId;
            config.EnableAutoOffsetStore = false;

            using (var schemaRegistry = new CachedSchemaRegistryClient(_configuration.SchemaRegistryConfig))
            using (var consumer = new ConsumerBuilder<TKey, TValue>(config)
                .SetValueDeserializer(new AvroDeserializer<TValue>(schemaRegistry).AsSyncOverAsync())
                .Build())
            {
                consumer.Subscribe(_topicName);

                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancellationTokenSource.Cancel();
                };

                try
                {
                    var count = 0;
                    while (true)
                    {
                        try
                        {
                            var cr = consumer.Consume(cancellationTokenSource.Token);
                            HandleMessageAsync(cr);
                            count++;

                            if (count == messageCount)
                            {
                                cancellationTokenSource.Cancel();
                            }
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

        protected abstract Task HandleMessageAsync(ConsumeResult<TKey, TValue> result);
    }
}