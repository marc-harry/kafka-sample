using Kafka.Common.Configuration;
using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure;
using Udemy;

namespace Kafka.Api.Services
{
    public class ReviewProducer : BaseProducer<long, Review>, IReviewProducer
    {
        public ReviewProducer(IGeneralConfiguration configuration) : base(configuration,
            TopicNames.NewReviews.GetDescription(), r => r.Id)
        {
        }
    }
}