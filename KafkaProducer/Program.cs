using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.IO;

namespace KafkaProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            const string topicName = "test-topic";

            var config = new List<KeyValuePair<string, string>>
                {new KeyValuePair<string, string>("bootstrap.servers", "localhost:9092")};

            Action<DeliveryReport<string, string>> handler = r => 
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");
            
            using (var producer = new ProducerBuilder<string, string>(config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
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

                    string key = null;
                    var val = text;

                    // split line if both key and value specified.
                    var index = text.IndexOf(" ", StringComparison.Ordinal);
                    if (index != -1)
                    {
                        key = text.Substring(0, index);
                        val = text.Substring(index + 1);
                    }

                    producer.Produce(topicName, new Message<string, string> {Key = key, Value = val}, handler);
                }

                producer.Flush();
            }
        }
    }
}
