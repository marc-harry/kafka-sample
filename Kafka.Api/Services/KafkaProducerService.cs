using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;

namespace Kafka.Api.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        public async Task<DeliveryResult<TKey, TValue>> SendAsync<TKey, TValue>(TopicNames topicName, TKey key, TValue message) where TValue : ISpecificRecord
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
                    return await producer.ProduceAsync(topicName.GetDescription(),
                        new Message<TKey, TValue> {Key = key, Value = message});
                }
                catch (ProduceException<TKey, TValue> exception)
                {
                    return exception.DeliveryResult;
                }
            }
        }
    }
}