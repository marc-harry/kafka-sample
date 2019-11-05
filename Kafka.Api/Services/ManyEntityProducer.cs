using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure.Json;

namespace Kafka.Api.Services
{
    public class ManyEntityProducer : MultiTypeJsonProducer<long>, IManyEntityProducer
    {
        public ManyEntityProducer(IGeneralConfiguration configuration) : base(configuration, "multi_entities")
        {
        }
    }
}