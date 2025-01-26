using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Grpc.Contracts;
using Itmo.Bebriki.Analytics.Presentation.Grpc.Mapper;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Controllers;

public class AnalyticsController : TaskAnalyticsService.TaskAnalyticsServiceBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ITransactionManager transactionManager,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public override async Task<GetAnalyticsResponse> GetAnalytics(
        GetAnalyticsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Requesting analytics for {request.TaskId}");

        TaskAnalytics analytics = await _transactionManager.RunAsync(
            async () => await _analyticsService.GetAnalyticsByIdAsync(request.TaskId, context.CancellationToken),
            context.CancellationToken,
            IsolationLevel.RepeatableRead);

        return new GetAnalyticsResponse
        {
            CreatedAt = analytics.CreatedAt.ToTimestamp(),
            LastUpdate = analytics.LastUpdate.ToTimestamp(),
            StartedAt = analytics.StartedAt.ToTimestamp(),
            TimeSpent = analytics.TimeSpent.ToDuration(),
            HighestPriority = JobTaskPriorityMapper.ToGrpc(analytics.HighestPriority),
            CurrentState = JobTaskStateMapper.ToGrpc(analytics.CurrentState),
            AmountOfAgreements = analytics.AmountOfAgreements,
            TotalUpdates = analytics.TotalUpdates,
            AmountOfUniqueAssignees = analytics.AmountOfUniqueAssignees,
            AmountOfUniqueDependencies = analytics.AmountOfUniqueDependencies,
        };
    }
}
