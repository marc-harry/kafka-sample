using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Common.Configuration;
using Kafka.Common.Infrastructure;
using Udemy;

namespace KafkaClient
{
    public class ReviewConsumer : BaseConsumer<long, Review>
    {
        public ReviewConsumer(GeneralConfiguration configuration) : base(configuration, "review-consumer",
            TopicNames.NewReviews.GetDescription())
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, Review> result)
        {
            Console.WriteLine($"Consumed message '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
}