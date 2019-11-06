using MediatR;

namespace Kafka.Common.Json
{
    public class CreateCompanyEvent : IRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}