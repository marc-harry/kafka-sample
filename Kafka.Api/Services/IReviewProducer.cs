using MHCore.Kafka.Infrastructure;
using Udemy;

namespace Kafka.Api.Services
{
    public interface IReviewProducer : IBaseProducer<long, Review>
    {
    }
}