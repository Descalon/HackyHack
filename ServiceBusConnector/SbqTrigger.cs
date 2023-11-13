using System;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServiceBusConnector
{
    public class SbqTrigger
    {
        private readonly ILogger<SbqTrigger> _logger;

        public SbqTrigger(ILogger<SbqTrigger> logger)
        {
            _logger = logger;

        }

        [Function(nameof(SbqTrigger))]
        public void Run([ServiceBusTrigger("sbq-msgt07-dev-001", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        }
    }
}
