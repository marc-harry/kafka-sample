using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Common.Configuration;
using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure;
using Udemy;

namespace KafkaClient
{
    public class ReviewConsumer : BaseConsumer<long, Review>
    {
        public ReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            TopicNames.NewReviews.GetDescription())
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, Review> result)
        {
            Console.WriteLine($"Consumed message '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
    
    public class TopReviewConsumer : BaseConsumer<Ignore, Review>
    {
        public TopReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            "TOP_REVIEWS")
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<Ignore, Review> result)
        {
            Console.WriteLine($"Consumed top_review message '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
}