using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace Kafka.Common.Configuration
{
    public interface IGeneralConfiguration
    {
        IGeneralConfiguration SetBootstrapServer(string url);

        IGeneralConfiguration SetSchemaRegistryServer(string url);

        IGeneralConfiguration SetProducerConfig(Action<ProducerConfig> config);

        IGeneralConfiguration SetConsumerConfig(Action<ConsumerConfig> config);

        IGeneralConfiguration SetAdminConfig(Action<AdminClientConfig> config);

        ProducerConfig GetProducerConfig();

        ConsumerConfig GetConsumerConfig();

        SchemaRegistryConfig GetSchemaRegistryConfig();

        AdminClientConfig GetAdminConfig();
    }
    
    public class GeneralConfiguration : IGeneralConfiguration
    {
        private ProducerConfig ProducerConfig { get; set; }

        private ConsumerConfig ConsumerConfig { get; set; }

        private AdminClientConfig AdminClientConfig { get; set; }
        
        private SchemaRegistryConfig SchemaRegistryConfig =>
            new SchemaRegistryConfig {SchemaRegistryUrl = SchemaRegistryUrl};

        private string SchemaRegistryUrl { get; set; }

        public IGeneralConfiguration SetBootstrapServer(string url)
        {
            ProducerConfig.BootstrapServers = url;
            ConsumerConfig.BootstrapServers = url;
            return this;
        }
        
        public IGeneralConfiguration SetSchemaRegistryServer(string url)
        {
            SchemaRegistryUrl = url;
            return this;
        }
        
        public IGeneralConfiguration SetProducerConfig(Action<ProducerConfig> config)
        {
            ProducerConfig = ProducerConfig ?? new ProducerConfig();
            config(ProducerConfig);
            return this;
        }
        
        public IGeneralConfiguration SetConsumerConfig(Action<ConsumerConfig> config)
        {
            ConsumerConfig = ConsumerConfig ?? new ConsumerConfig();
            config(ConsumerConfig);
            return this;
        }
        
        public IGeneralConfiguration SetAdminConfig(Action<AdminClientConfig> config)
        {
            AdminClientConfig = AdminClientConfig ?? new AdminClientConfig();
            config(AdminClientConfig);
            return this;
        }

        public ProducerConfig GetProducerConfig()
        {
            return ProducerConfig;
        }

        public ConsumerConfig GetConsumerConfig()
        {
            return ConsumerConfig;
        }

        public SchemaRegistryConfig GetSchemaRegistryConfig()
        {
            return SchemaRegistryConfig;
        }

        public AdminClientConfig GetAdminConfig()
        {
            return AdminClientConfig;
        }
    }

    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, Action<IGeneralConfiguration> kafkaConfig)
        {
            var config = new GeneralConfiguration()
                .SetProducerConfig(c => c.BootstrapServers = null)
                .SetConsumerConfig(c =>
                {
                    c.BootstrapServers = null;
                    c.StatisticsIntervalMs = 60000;
                    c.SessionTimeoutMs = 6000;
                });

            kafkaConfig(config);

            if (string.IsNullOrWhiteSpace(config.GetProducerConfig().BootstrapServers) ||
                string.IsNullOrWhiteSpace(config.GetConsumerConfig().BootstrapServers))
            {
                throw new Exception("Bootstrap servers url not provided!");
            }

            if (string.IsNullOrWhiteSpace(config.GetSchemaRegistryConfig().SchemaRegistryUrl))
            {
                throw new Exception("Schema registry url not provided!");
            }

            services.AddSingleton(config);
            
            return services;
        }
    }
}