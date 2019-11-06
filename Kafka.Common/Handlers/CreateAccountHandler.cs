using System;
using System.Threading;
using System.Threading.Tasks;
using Kafka.Common.Json;
using MediatR;

namespace Kafka.Common.Handlers
{
    public class CreateAccountHandler : IRequestHandler<CreateCompanyEvent>
    {
        public Task<Unit> Handle(CreateCompanyEvent request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumed message: '{request.Name}' at: 'dispatched message'.");
            
            return Task.FromResult(Unit.Value);
        }
    }
}