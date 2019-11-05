using MHCore.Kafka.Infrastructure.Json;

namespace Kafka.Api.Services
{
    public interface IManyEntityProducer : IMultiTypeProducer<long>
    {
    }
}