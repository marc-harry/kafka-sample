using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Common.Configuration
{
    public class GeneralConfiguration
    {
        public ProducerConfig ProducerConfig { get; set; }

        public ConsumerConfig ConsumerConfig { get; set; }
        
        public SchemaRegistryConfig SchemaRegistryConfig =>
            new SchemaRegistryConfig {SchemaRegistryUrl = SchemaRegistryUrl};

        private string SchemaRegistryUrl { get; set; }

        public GeneralConfiguration SetBootstrapServer(string url)
        {
            ProducerConfig.BootstrapServers = url;
            ConsumerConfig.BootstrapServers = url;
            return this;
        }
        
        public GeneralConfiguration SetSchemaRegistryServer(string url)
        {
            SchemaRegistryUrl = url;
            return this;
        }
    }

    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, Action<GeneralConfiguration> kafkaConfig)
        {
            var config = new GeneralConfiguration
            {
                ConsumerConfig = new ConsumerConfig
                {
                    BootstrapServers = null,
                    StatisticsIntervalMs = 60000,
                    SessionTimeoutMs = 6000
                },
                ProducerConfig = new ProducerConfig
                {
                    BootstrapServers = null
                }
            };

            kafkaConfig(config);

            if (string.IsNullOrWhiteSpace(config.ProducerConfig.BootstrapServers) ||
                string.IsNullOrWhiteSpace(config.ConsumerConfig.BootstrapServers))
            {
                throw new Exception("Bootstrap servers url not provided!");
            }

            if (string.IsNullOrWhiteSpace(config.SchemaRegistryConfig.SchemaRegistryUrl))
            {
                throw new Exception("Schema registry url not provided!");
            }

            services.AddSingleton(config);
            
            return services;
        }
    }
}