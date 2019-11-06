using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Kafka.Common.Configuration;
using Kafka.Common.Json;
using MediatR;
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
        public JsonReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer1",
            new [] { TopicNames.NewReviews.GetDescription() })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, ReviewEntity> result)
        {
            Console.WriteLine($"Consumed message: '{result.Value.Title}' at: '{result.TopicPartitionOffset}' on topic: {result.Topic}.");
            return Task.CompletedTask;
        }
    }
    
    public class ManyJsonConsumer : MultiTypeJsonConsumer<long>
    {
        private readonly IMediator _mediator;

        public ManyJsonConsumer(IGeneralConfiguration configuration, IMediator mediator) : base(configuration, "review-consumer2",
            new [] { "multi_entities" })
        {
            _mediator = mediator;
        }

        protected override Task HandleMessageAsync(ConsumeResult<long, object> result)
        {
            // Either handle messages by sending to registered mediator handlers
            if (result.Value is IRequest request)
            { 
                return _mediator.Send(request);
            }

            // Or manually handle events by finding type and handling
            switch (result.Value)
            {
                case CreateCompanyEvent companyEvent:
                    Console.WriteLine($"Consumed message: '{companyEvent.Name}' at: '{result.TopicPartitionOffset}' on topic: {result.Topic}.");
                    break;
                case ReviewEntity review:
                    Console.WriteLine($"Consumed message: '{review.Title}' at: '{result.TopicPartitionOffset}' on topic: {result.Topic}.");
                    break;
            }

            return Task.CompletedTask;
        }
    }
    
    public class TopReviewConsumer : SingleTypeJsonConsumer<Ignore, ReviewEntity>
    {
        public TopReviewConsumer(IGeneralConfiguration configuration) : base(configuration, "review-consumer3",
            new [] { "TOP_REVIEWS" })
        {
        }

        protected override Task HandleMessageAsync(ConsumeResult<Ignore, ReviewEntity> result)
        {
            Console.WriteLine($"Consumed message: '{result.Value.Rating}' at: '{result.TopicPartitionOffset}' on topic: {result.Topic}.");
            return Task.CompletedTask;
        }
    }
}