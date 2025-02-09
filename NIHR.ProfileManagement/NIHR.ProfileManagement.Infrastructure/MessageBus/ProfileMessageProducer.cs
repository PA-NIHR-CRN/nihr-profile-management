﻿using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.ProfileManagement.Domain.Abstractions;
using NIHR.ProfileManagement.Domain.Configuration;
using NIHR.ProfileManagement.Domain.Models;

namespace NIHR.ProfileManagement.Infrastructure.MessageBus
{
    public class ProfileKafkaMessageProducer : IProfileEventMessagePublisher, INsipMessageHelper
    {
        private readonly ILogger<ProfileKafkaMessageProducer> _logger;
        private readonly MessageBusSettings _settings;

        public ProfileKafkaMessageProducer(ILogger<ProfileKafkaMessageProducer> logger,
            IOptions<MessageBusSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            _logger.LogInformation($"MSK settings BootstrapServers: {_settings.BootstrapServers}. Topic: {_settings.Topic}");
        }

        public NsipMessage<string> Prepare(string eventType, string sourceSystem, string eventData)
        {
            var nsipMessage = new NsipMessage<string>(eventData)
            {
                NsipEventId = Guid.NewGuid().ToString(),
                NsipEventSourceSystemId = sourceSystem,
                NsipEventType = eventType,
            };

            return nsipMessage;
        }

        public async Task PrepareAndPublishAsync(string eventType,
            string sourceSystemName,
            string payload,
            CancellationToken cancellationToken)
        {
            var nsipMessage = Prepare(eventType, sourceSystemName, payload);

            await Publish(nsipMessage, cancellationToken);
        }

        private async Task Publish<TEventType>(NsipMessage<TEventType> nsipMessage,
            CancellationToken cancellationToken)
        {
            var nsipMessageJson = System.Text.Json.JsonSerializer.Serialize(nsipMessage);

            await Publish(nsipMessageJson, cancellationToken);
        }

        public async Task Publish(string payload, CancellationToken cancellationToken)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers
            };

            using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                var msg = new Message<Null, string>() { Value = payload };

                _logger.LogInformation($"Producer: About to publish {payload}");

                var deliveryReport = await producer.ProduceAsync(_settings.Topic, msg, cancellationToken: cancellationToken);

                _logger.LogInformation("Delivered message to Topic={Topic} " +
                    "Offset={Offset} " +
                    "Key={Key} " +
                    "Partition={Partition} " +
                    "Status={Status} " +
                    "Value={Value}",
                    _settings.Topic,
                    deliveryReport.Offset.Value,
                    "PoC" ?? "NULL",
                    deliveryReport.TopicPartition.Partition.Value,
                    deliveryReport.Status,
                    payload);
            }
        }
    }
}
