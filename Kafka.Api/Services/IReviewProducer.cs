using Kafka.Common.Json;
using MHCore.Kafka.Infrastructure;

namespace Kafka.Api.Services
{
    public interface IReviewProducer : IBaseProducer<long, ReviewEntity>
    {
    }
}