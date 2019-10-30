using System.Threading;
using System.Threading.Tasks;
using Avro.Specific;

namespace Kafka.Common.Infrastructure
{
    public interface IBaseConsumer<TKey, TValue> where TValue : ISpecificRecord
    {
        Task ConsumeAsync(CancellationTokenSource cts);
    }
}