using System;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Hackvip.Domain;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServiceBusConnector
{
    public record ServiceBusMessage(
        string AanvragerKey,
        string BerichtType,
        DateTime DatumDagtekening,
        bool KgbVariant,
        DateTime ReactieDatum,
        int ToeslagJaar
    ){
        public override string ToString()
            =>$@"{{
AanvragerKey = {AanvragerKey},
BerichtType = {BerichtType},
DatumDagtekening = {DatumDagtekening},
KgbVariant = {KgbVariant},
ReactieDatum = {ReactieDatum},
ToeslagJaar = {ToeslagJaar}
            }}";
    }
    public class SbqTrigger
    {
        private readonly ILogger<SbqTrigger> _logger;

        public SbqTrigger(ILogger<SbqTrigger> logger)
        {
            _logger = logger;

        }

        private static BerichtType ParseBerichtType(string v) => v switch {
            "vraagbrief" => BerichtType.Vraag,
            "rappelbrief" => BerichtType.Rappel,
            _ => BerichtType.Beschikking,
        };

        [Function(nameof(SbqTrigger))]
        public async Task Run([ServiceBusTrigger("sbq-msgt07-dev-001", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage rawMessage)
        {
            _logger.LogInformation("Message ID: {id}", rawMessage.MessageId);
            _logger.LogInformation("Message Body: {body}", rawMessage.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", rawMessage.ContentType);
            var message = JsonSerializer.Deserialize<ServiceBusMessage>(System.Text.Encoding.UTF8.GetString(rawMessage.Body), new JsonSerializerOptions(){
                PropertyNameCaseInsensitive = true
            });
            _logger.LogInformation(message?.ToString() ?? "deserialization error");
            if(message == null) return;
            using var channel = GrpcChannel.ForAddress("http://localhost:5244");

            var client = new Ingress.IngressClient(channel);
            var request = new PublishDataMessage(){
                AanvragerKey = message.AanvragerKey,
                BerichtType = ParseBerichtType(message.BerichtType),
                DatumDagtekening = Timestamp.FromDateTime(message.DatumDagtekening.ToUniversalTime()),
                KgbVariant = message.KgbVariant,
                ReactieDatum = Timestamp.FromDateTime(message.ReactieDatum.ToUniversalTime()),
                ToeslagJaar = message.ToeslagJaar
            };
            await client.PublishDataAsync(request);
        }
    }
}
