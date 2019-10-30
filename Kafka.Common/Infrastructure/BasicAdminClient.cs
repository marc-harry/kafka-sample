using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kafka.Common.Configuration;

namespace Kafka.Common.Infrastructure
{
    public class BasicAdminClient
    {
        private readonly IGeneralConfiguration _configuration;

        public BasicAdminClient(IGeneralConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task CreateTopicAsync(string topicName, int partitions = 3, short replicationFactor = 1)
        {
            await PerformAdminOperation(async client =>
            {
                await client.CreateTopicsAsync(new[]
                {
                    new TopicSpecification
                        {Name = topicName, NumPartitions = partitions, ReplicationFactor = replicationFactor}
                });
            });
        }
        
        public async Task DeleteTopicAsync(string topicName)
        {
            await PerformAdminOperation(async client =>
            {
                await client.DeleteTopicsAsync(new[]
                {
                    topicName
                });
            });
        }

        private async Task PerformAdminOperation(Func<IAdminClient, Task> operation)
        {
            using (var adminClient = new AdminClientBuilder(_configuration.GetAdminConfig())
                .Build())
            {
                await operation(adminClient);
            }
        }
    }
}