using System.Threading.Tasks;

namespace Kafka.Common.Infrastructure
{
    public interface IBasicAdminClient
    {
        Task CreateTopicAsync(string topicName, int partitions = 3, short replicationFactor = 1);

        Task DeleteTopicAsync(string topicName);
    }
}