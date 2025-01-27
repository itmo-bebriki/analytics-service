using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Contracts;
using Itmo.Bebriki.Analytics.Grpc.Enums;
using Itmo.Bebriki.Analytics.Presentation.Grpc.Mappers;
using Microsoft.Extensions.Logging;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Controllers;

public class AnalyticsController : TaskAnalyticsService.TaskAnalyticsServiceBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    public override async Task<GetAnalyticsResponse> GetAnalytics(
        GetAnalyticsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Requesting analytics for {request.TaskId}");

        TaskAnalytics? analytics = await _analyticsService.GetAnalyticsByIdAsync(request.TaskId, context.CancellationToken);

        if (analytics == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Task {request.TaskId} not found"));

        return new GetAnalyticsResponse
        {
            CreatedAt = analytics.CreatedAt?.ToTimestamp(),
            LastUpdate = analytics.LastUpdate?.ToTimestamp(),
            StartedAt = analytics.StartedAt?.ToTimestamp(),
            TimeSpent = analytics.TimeSpent?.ToDuration(),
            HighestPriority = analytics.HighestPriority is not null ? JobTaskPriorityMapper.ToGrpc((Application.Models.JobTask.JobTaskPriority)analytics.HighestPriority) : JobTaskPriority.Unspecified,
            CurrentState = analytics.CurrentState is not null ? JobTaskStateMapper.ToGrpc((Application.Models.JobTask.JobTaskState)analytics.CurrentState) : JobTaskState.Unspecified,
            AmountOfAgreements = analytics.AmountOfAgreements,
            TotalUpdates = analytics.TotalUpdates,
            AmountOfUniqueAssignees = analytics.AmountOfUniqueAssignees,
            AmountOfUniqueDependencies = analytics.AmountOfUniqueDependencies,
        };
    }
}
