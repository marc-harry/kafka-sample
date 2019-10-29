using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;

namespace Kafka.Common.Infrastructure
{
    public abstract class BaseProducer<TKey, TValue> : IBaseProducer<TKey, TValue> where TValue : ISpecificRecord
    {
        private readonly string _topicName;
        private readonly Func<TValue, TKey> _key;
        
        protected BaseProducer(string topicName, Func<TValue, TKey> keyAccessor = null)
        {
            _topicName = topicName;
            if (typeof(TKey) != typeof(Null))
            {
                _key = keyAccessor;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns cref="DeliveryResult{TKey,TValue}"></returns>
        public async Task<DeliveryResult<TKey, TValue>> ProduceAsync(TValue message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = GeneralConfiguration.BootstrapServer
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(SchemaConfiguration.SchemaRegistryConfig))
            using (var producer = new ProducerBuilder<TKey, TValue>(config)
                .SetValueSerializer(new AvroSerializer<TValue>(schemaRegistry))
                .Build())
            {
                try
                {
                    var messageToSend = typeof(TKey) == typeof(Null)
                        ? new Message<TKey, TValue> {Value = message}
                        : new Message<TKey, TValue> {Key = _key(message), Value = message};
                    return await producer.ProduceAsync(_topicName, messageToSend);
                }
                catch (ProduceException<TKey, TValue> exception)
                {
                    return exception.DeliveryResult;
                }
            }
        }
        
        public async Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceManyAsync(IEnumerable<TValue> messages)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = GeneralConfiguration.BootstrapServer,
                QueueBufferingMaxMessages = 2000000,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 500,
                LingerMs = 5
            };

            DeliveryResult<TKey, TValue>[] reports;
            using (var schemaRegistry = new CachedSchemaRegistryClient(SchemaConfiguration.SchemaRegistryConfig))
            using (var producer = new ProducerBuilder<TKey, TValue>(config)
                .SetValueSerializer(new AvroSerializer<TValue>(schemaRegistry))
                .Build())
            {
                var tasks = new List<Task<DeliveryResult<TKey, TValue>>>();
                foreach (var message in messages)
                {
                    var messageToSend = typeof(TKey) == typeof(Null)
                        ? new Message<TKey, TValue> {Value = message}
                        : new Message<TKey, TValue> {Key = _key(message), Value = message};
                    tasks.Add(producer.ProduceAsync(_topicName, messageToSend));
                }

                reports = await Task.WhenAll(tasks);
                producer.Flush(TimeSpan.FromMilliseconds(100));
            }

            return reports;
        }
    }
}