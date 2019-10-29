using Kafka.Common.Configuration;
using Kafka.Common.Infrastructure;
using Udemy;

namespace Kafka.Api.Services
{
    public class ReviewProducer : BaseProducer<long, Review>, IReviewProducer
    {
        public ReviewProducer() : base(TopicNames.NewReviews.GetDescription(), r => r.Id)
        {
        }
    }
}