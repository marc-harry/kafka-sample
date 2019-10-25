using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Kafka.Common.Configuration;
using Udemy;

namespace KafkaProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string topicName = "test-topic-reviews";

            var config = new ProducerConfig
            {
                BootstrapServers = GeneralConfiguration.BootstrapServer
            };

            using (var schemaRegistry = new CachedSchemaRegistryClient(SchemaConfiguration.SchemaRegistryConfig))
            using (var producer = new ProducerBuilder<long, Review>(config)
                .SetValueSerializer(new AvroSerializer<Review>(schemaRegistry))
                .Build())
            {
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("To create a kafka message with UTF-8 encoded key and value:");
                Console.WriteLine("> key value<Enter>");
                Console.WriteLine("To create a kafka message with a null key and UTF-8 encoded value:");
                Console.WriteLine("> value<enter>");
                Console.WriteLine("Ctrl-C to quit.\n");

                var cancelled = false;
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                while (!cancelled)
                {
                    Console.Write("> ");

                    string text;
                    try
                    {
                        text = Console.ReadLine();
                    }
                    catch (IOException)
                    {
                        // IO exception is thrown when ConsoleCancelEventArgs.Cancel == true.
                        break;
                    }
                    if (text == null)
                    {
                        // Console returned null before 
                        // the CancelKeyPress was treated
                        break;
                    }

                    var review = new Review
                    {
                        Id = Convert.ToInt64(new Random().Next()),
                        Title = text,
                        Rating = "5",
                        Created = DateTime.UtcNow.Ticks,
                        Modified = DateTime.UtcNow.Ticks,
                        User = new User
                        {
                            Name = "Admin",
                            Title = "Admin",
                            DisplayName = "Admin"
                        },
                        Course = new Course
                        {
                            Id = 234356,
                            Title = "Learn Kafka",
                            Url = "localhost.dev"
                        }
                    };

                    producer.ProduceAsync(topicName, new Message<long, Review> { Key = review.Id, Value = review })
                        .ContinueWith(task => Console.WriteLine(
                            task.IsFaulted
                                ? $"error producing message: {task.Exception.Message}"
                                : $"produced to: {task.Result.TopicPartitionOffset}"));

                    Console.WriteLine();
                }

                producer.Flush();
            }
        }
    }
}
