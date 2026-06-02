using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace MicroservicesWithKafka.Services.Kafka
{
    public class KafkaProducerService
    {
        private readonly string _bootstrapServers;

        public KafkaProducerService(IConfiguration configuration)
        {
            _bootstrapServers = configuration["Kafka:BootstrapServers"]
                ?? throw new ArgumentNullException("Kafka:BootstrapServers is missing in configuration");
        }

        // Topic name is now passed in per call
        public async Task SendMessageAsync(string topicName, string key, string message)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            using var producer = new ProducerBuilder<string, string>(config).Build();

            try
            {
                var result = await producer.ProduceAsync(topicName, new Message<string, string>
                {
                    Key = key,
                    Value = message
                });

                Console.WriteLine($"Kafka message sent to topic '{topicName}' at {result.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"Kafka produce failed: {ex.Error.Reason}");
            }
        }
    }
}
