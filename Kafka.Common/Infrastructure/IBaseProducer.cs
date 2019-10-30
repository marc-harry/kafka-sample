using System.Collections.Generic;
using System.Threading.Tasks;
using Avro.Specific;
using Confluent.Kafka;

namespace Kafka.Common.Infrastructure
{
    public interface IBaseProducer<TKey, TValue> where TValue : ISpecificRecord
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns cref="DeliveryResult{TKey,TValue}"></returns>
        Task<DeliveryResult<TKey, TValue>> ProduceAsync(TValue message);

        Task<IEnumerable<DeliveryResult<TKey, TValue>>> ProduceManyAsync(IEnumerable<TValue> messages);

        void ProduceMany(IEnumerable<TValue> messages);
    }
}