using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Common.Configuration;
using Kafka.Common.Json;
using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure;
using MHCore.Kafka.Infrastructure.Json;
using Udemy;

namespace KafkaClient
{
    public class ReviewConsumer : BaseConsumer<long, Review>
    {
        public ReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            new [] { TopicNames.NewReviews.GetDescription() })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, Review> result)
        {
            Console.WriteLine($"Consumed message '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
    
    public class JsonReviewConsumer : SingleTypeJsonConsumer<long, ReviewEntity>
    {
        public JsonReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            new [] { TopicNames.NewReviews.GetDescription() })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, ReviewEntity> result)
        {
            Console.WriteLine($"Consumed message: '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
    
    public class ManyJsonConsumer : MultiTypeJsonConsumer<long>
    {
        public ManyJsonConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            new [] { "multi_entities" })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, object> result)
        {
            switch (result.Value)
            {
                case CreateCompanyEvent companyEvent:
                    Console.WriteLine($"Consumed message: '{companyEvent.Name}' at: '{result.TopicPartitionOffset}'.");
                    break;
                case ReviewEntity review:
                    Console.WriteLine($"Consumed message: '{review.Title}' at: '{result.TopicPartitionOffset}'.");
                    break;
            }

            return Task.CompletedTask;
        }
    }
    
    public class TopReviewConsumer : SingleTypeJsonConsumer<Ignore, ReviewEntity>
    {
        public TopReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer",
            new [] { "TOP_REVIEWS" })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<Ignore, ReviewEntity> result)
        {
            Console.WriteLine($"Consumed message: '{result.Value.Title}' at: '{result.TopicPartitionOffset}'.");
            return Task.CompletedTask;
        }
    }
}