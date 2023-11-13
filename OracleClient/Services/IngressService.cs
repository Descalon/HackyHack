using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Hackvip.Domain;
using Hackvip.Models;

namespace Hackvip.Services;

public class IngressService(ILogger<IngressService> logger, OracleRequestService requestService, Repository.RepositoryClient repoClient) : Ingress.IngressBase {
    private readonly ILogger<IngressService> _logger = logger;
    private readonly OracleRequestService _requestService = requestService;
    private readonly Repository.RepositoryClient _repoClient = repoClient;

    public override async Task<PublishReply> PublishData(PublishDataMessage request, ServerCallContext context)
    {
        var burger = await _requestService.GetBurger(Guid.Parse(request.AanvragerKey));
        var _ = request.BerichtType switch
        {
            BerichtType.Vraag => await _repoClient.PostVraagDataAsync(new() { Data = VraagBriefFromRequest(request, burger) }),
            BerichtType.Rappel => await _repoClient.PostRappelDataAsync(new() { Data = RappelBriefFromRequest(request) }),
            BerichtType.Beschikking => await _repoClient.PostBeschikkingDataAsync(new() { Data = BeschikkingBriefFromRequest(request) }),
            _ => throw new NotImplementedException(),
        };
        
        return new PublishReply();
    }

    private VraagBrief VraagBriefFromRequest(PublishDataMessage request, BurgerModel b) => new()
    {
        AanvragerKey = request.AanvragerKey,
        Burger = b.ToProtobufModel(),
        KgbVariant = request.KgbVariant,
        ToeslagJaar = request.ToeslagJaar,
        Td = new TemplateData()
        {
            DatumDagtekening = Timestamp.FromDateTime(DateTime.Now),
            Kenmerk = "",
        }
    };
    private Rappelbrief RappelBriefFromRequest(PublishDataMessage request) => new()
    {
        AanvragerKey = request.AanvragerKey,
        Td = new TemplateData()
        {
            Kenmerk = "",
            DatumDagtekening = Timestamp.FromDateTime(DateTime.Now),
        }
    };
    private BeschikkingBrief BeschikkingBriefFromRequest(PublishDataMessage request) => new()
    {
        AanvragerKey = request.AanvragerKey,
        Td = new TemplateData()
        {
            DatumDagtekening = Timestamp.FromDateTime(DateTime.Now),
        }
    };
}
