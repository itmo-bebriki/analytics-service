using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Mappers;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application;

public class AnalyticsService : IAnalyticsService
{
    private readonly IPersistenceContext _context;

    public AnalyticsService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<TaskAnalytics?> GetAnalyticsByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _context.AnalyticsRepository.QueryAsync(
            new FetchAnalyticsQuery(id),
            cancellationToken);
    }

    public async Task<PagedHistoryEvents> GetHistoryByIdAsync(FetchHistoryCommand command, CancellationToken cancellationToken)
    {
        HashSet<PayloadEvent> events = await _context.EventHistoryRepository.QueryAsync(
            new FetchQuery(
                Ids: command.Ids,
                Types: command.Types,
                FromTimestamp: command.FromTimestamp,
                ToTimestamp: command.ToTimestamp,
                PageSize: command.PageSize),
            cancellationToken)
            .Select(e => new PayloadEvent(
                Id: e.Id,
                EventType: e.EventType,
                Timestamp: e.Timestamp,
                Command: PayloadMapper.ToCommand(e.Payload, e.EventType)))
            .ToHashSetAsync(cancellationToken);

        long? cursor = events.Count == command.PageSize && events.Count > 0
            ? events.Last().Id
            : null;

        return new PagedHistoryEvents(cursor, events);
    }

    public async Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await _context.AnalyticsRepository.UpsertAsync(
            new UpsertAnalyticsQuery(
                Id: command.JobTaskId,
                CreatedAt: command.CreatedAt,
                UpdatedAt: null,
                StartedAt: null,
                TimeSpent: null,
                Priority: command.Priority,
                State: JobTaskState.None,
                AmountOfAgreements: 0,
                TotalUpdates: 1),
            cancellationToken);

        await _context.AnalyticsRepository.AddDependencyAsync(
            new AddDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.DependsOnIds),
            cancellationToken);

        await _context.AnalyticsRepository.AddAssigneeAsync(
            new AddAssigneeQuery(
                Id: command.JobTaskId,
                AssigneeId: command.AssigneeId),
            cancellationToken);
    }

    public async Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken)
    {
        TaskAnalytics? current = await _context.AnalyticsRepository.QueryAsync(
            new FetchAnalyticsQuery(command.JobTaskId),
            cancellationToken);

        await _context.AnalyticsRepository.UpsertAsync(
            new UpsertAnalyticsQuery(
                Id: command.JobTaskId,
                UpdatedAt: command.UpdatedAt,
                StartedAt: command.State is >= JobTaskState.InProgress ? command.UpdatedAt : null,
                TimeSpent: command.State is >= JobTaskState.Done ? command.UpdatedAt - (current is null ? null : current.StartedAt ?? command.UpdatedAt) : null,
                CreatedAt: null,
                Priority: command.Priority > current?.HighestPriority ? command.Priority : null,
                State: command.State,
                AmountOfAgreements: current?.AmountOfAgreements + (command.IsAgreed.Equals(null) ? 0 : 1),
                TotalUpdates: current?.TotalUpdates + 1),
            cancellationToken);

        if (command.AssigneeId.HasValue)
        {
            await _context.AnalyticsRepository.AddAssigneeAsync(
                new AddAssigneeQuery(command.JobTaskId, command.AssigneeId.Value),
                cancellationToken);
        }
    }

    public async Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _context.AnalyticsRepository.AddDependencyAsync(
            new AddDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.Dependencies),
            cancellationToken);
    }

    public async Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _context.AnalyticsRepository.RemoveDependencyAsync(
            new RemoveDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.Dependencies),
            cancellationToken);
    }
}
