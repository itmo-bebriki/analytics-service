using Grpc.Core;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Bebriki.Analytics.Contracts;
using Itmo.Bebriki.Analytics.Presentation.Grpc.Mappers;
using Microsoft.Extensions.Logging;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Controllers;

public class EventHistoryController : HistoryAnalyticsService.HistoryAnalyticsServiceBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public EventHistoryController(
        IAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public override async Task<GetHistoryResponse> GetHistory(
        GetHistoryRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Getting history for {request.Ids}");

        PagedHistoryEvents pagedHistoryEvents = await _analyticsService.GetHistoryByIdAsync(
            new FetchHistoryCommand(
                JobTaskIds: request.Ids.ToArray(),
                Types: request.EventTypes.Select(EventTypeMapper.ToInternal).ToArray(),
                FromTimestamp: request.FromTimestamp?.ToDateTimeOffset(),
                ToTimestamp: request.ToTimestamp?.ToDateTimeOffset(),
                PageSize: request.PageSize,
                Cursor: request.Cursor),
            context.CancellationToken);

        return new GetHistoryResponse
        {
            Cursor = pagedHistoryEvents.Cursor ?? -1,
            Payloads = { pagedHistoryEvents.Events.Select(PayloadMapper.FromInternal).ToArray() },
        };
    }
}
