using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KafkaProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            string topicName = "testTopik";

            var config = new Dictionary<string, object> { { "bootstrap.servers", "127.0.0.1" } };

            using (var producer = new Producer<string, string>(config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8)))
            {
                Console.WriteLine("\n-----------------------------------------------------------------------");
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
                    string val = text;

                    // split line if both key and value specified.
                    int index = text.IndexOf(" ");
                    if (index != -1)
                    {
                        key = text.Substring(0, index);
                        val = text.Substring(index + 1);
                    }

                    // Calling .Result on the asynchronous produce request below causes it to
                    // block until it completes. Generally, you should avoid producing
                    // synchronously because this has a huge impact on throughput. For this
                    // interactive console example though, it's what we want.
                    var deliveryReport = producer.ProduceAsync(topicName, key, val).Result;
                    Console.WriteLine(
                        deliveryReport.Error.Code == ErrorCode.NoError
                            ? $"delivered to: {deliveryReport.TopicPartitionOffset}"
                            : $"failed to deliver message: {deliveryReport.Error.Reason}"
                    );
                }

                // Since we are producing synchronously, at this point there will be no messages
                // in flight and no delivery reports waiting to be acknowledged, so there is no
                // need to call producer.Flush before disposing the producer, as you typically 
                // would.
            }
        }
    }
}
