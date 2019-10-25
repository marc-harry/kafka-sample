using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;
using Kafka.Common.Configuration;

namespace Kafka.Api.Services
{
    public interface IKafkaProducerService
    {
        Task<DeliveryResult<TKey, TValue>> SendAsync<TKey, TValue>(TopicNames topicName, TKey key, TValue message)
            where TValue : ISpecificRecord;
    }
}