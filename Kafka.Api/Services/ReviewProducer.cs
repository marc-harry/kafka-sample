using Kafka.Common.Configuration;
using Kafka.Common.Json;
using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure.Json;

namespace Kafka.Api.Services
{
    public class ReviewProducer : SingleTypeJsonProducer<long, ReviewEntity>, IReviewProducer
    {
        public ReviewProducer(IGeneralConfiguration configuration) : base(configuration,
            TopicNames.NewReviews.GetDescription(), r => r.Id)
        {
        }
    }
}