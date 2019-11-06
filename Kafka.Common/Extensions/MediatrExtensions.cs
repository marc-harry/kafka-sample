using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Kafka.Common.Extensions
{
    public static class MediatrExtensions
    {
        public static async Task<object> Send(this IMediator mediator, object request, CancellationToken cancellationToken = default)
        {
            return await mediator.Send((dynamic)request, cancellationToken);
        }
    }
}