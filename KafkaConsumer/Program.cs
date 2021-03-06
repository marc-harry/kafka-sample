﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kafka.Common.Configuration;
using Kafka.Common.Handlers;
using MediatR;
using MHCore.Kafka.Configuration;
using MHCore.Kafka.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace KafkaClient
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Consumer starting up!");

            var serviceProvider = new ServiceCollection()
                .AddMediatR(typeof(CreateAccountHandler).Assembly)
                .BuildServiceProvider();

            var config = new GeneralConfiguration()
                .SetConsumerConfig(c =>
                {
                    c.BootstrapServers = "localhost:9092";
                    c.StatisticsIntervalMs = 60000;
                    c.SessionTimeoutMs = 6000;
                    c.EnableAutoOffsetStore = false;
                    c.AutoCommitIntervalMs = 60000; // Every 1 minute
                    c.QueuedMinMessages = 1000000;
                })
                .SetProducerConfig(c => c.BootstrapServers = "localhost:9092")
                .SetSchemaRegistryServer("localhost:8081")
                .SetAdminConfig(c => c.BootstrapServers = "localhost:9092")
                .SetSerializerSettings(c =>
                {
                    c.TypeNameHandling = TypeNameHandling.None;
                    c.Formatting = Formatting.None;
                });

            var adminClient = new BasicAdminClient(config);

//            await SetupTopic(adminClient, TopicNames.NewReviews.GetDescription());
//            await SetupTopic(adminClient, "multi_entities");

            var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            var taskOne = StartJsonConsumer(config, cts);
            var taskTwo = StartMultiJsonConsumer(config, serviceProvider, cts);
            var taskThree = StartTopReviewsConsumer(config, cts);

            await Task.WhenAll(taskOne, taskTwo, taskThree);
        }

        private static Task StartJsonConsumer(IGeneralConfiguration config, CancellationTokenSource cts)
        {
            var consumer = new JsonReviewConsumer(config);
            Console.WriteLine("Consumer listening:");
            return Task.Run(() => consumer.ConsumeAsync(cts));
        }
        
        private static Task StartMultiJsonConsumer(IGeneralConfiguration config, IServiceProvider serviceProvider, CancellationTokenSource cts)
        {
            var consumer = new ManyJsonConsumer(config, serviceProvider.GetService<IMediator>());
            Console.WriteLine("ManyConsumer listening:");
            return Task.Run(() => consumer.ConsumeAsync(cts));
        }
        
        private static Task StartTopReviewsConsumer(IGeneralConfiguration config, CancellationTokenSource cts)
        {
//            var specificConfig = new GeneralConfiguration()
//                .SetConsumerConfig(c =>
//                {
//                    c.BootstrapServers = "localhost:9092";
//                    c.StatisticsIntervalMs = 60000;
//                    c.SessionTimeoutMs = 6000;
//                    c.EnableAutoOffsetStore = false;
//                    c.AutoCommitIntervalMs = 60000; // Every 1 minute
//                    c.QueuedMinMessages = 1000000;
//                    c.AutoOffsetReset = AutoOffsetReset.Earliest;
//                }).SetSerializerSettings(c =>
//                {
//                    c.TypeNameHandling = TypeNameHandling.None;
//                    c.Formatting = Formatting.None;
//                });
            var consumer = new TopReviewConsumer(config);
            Console.WriteLine("TopReviewConsumer listening:");
            return Task.Run(() => consumer.ConsumeAsync(cts));
        }

        private static async Task SetupTopic(IBasicAdminClient adminClient, string topicName)
        {
            try
            {
                Console.WriteLine($"Deleting topic - {topicName}");
                await adminClient.DeleteTopicAsync(topicName, TimeSpan.FromSeconds(5), null);
                Console.WriteLine($"Topic deleted - {topicName}");
            }
            catch (DeleteTopicsException e)
            {
                Console.WriteLine($"Failed to deleted topic Message: {e.Message}");
            }

            Console.WriteLine($"Creating topic - {topicName}");
            await adminClient.CreateTopicAsync(topicName);
        }
    }
}
