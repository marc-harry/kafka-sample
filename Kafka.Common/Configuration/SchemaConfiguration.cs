using Confluent.SchemaRegistry;

namespace Kafka.Common.Configuration
{
    public static class SchemaConfiguration
    {
        public const string SchemaRegistryUrl = "localhost:8081";

        public static SchemaRegistryConfig SchemaRegistryConfig = new SchemaRegistryConfig
        {
            SchemaRegistryUrl = SchemaRegistryUrl
        };
    }
}