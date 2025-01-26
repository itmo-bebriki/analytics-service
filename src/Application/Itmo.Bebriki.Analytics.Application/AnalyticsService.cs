using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Application;

public class AnalyticsService : IAnalyticsService
{
    private readonly IAnalyticsRepository _analyticsRepository;

    public AnalyticsService(IAnalyticsRepository analyticsRepository)
    {
        _analyticsRepository = analyticsRepository;
    }

    public async Task<TaskAnalytics> GetAnalyticsByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _analyticsRepository.QueryAsync(
            new FetchAnalyticsQuery(id),
            cancellationToken);
    }

    public async Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await _analyticsRepository.UpsertAsync(
            new UpsertAnalyticsQuery(
                Id: command.JobTaskId,
                CreatedAt: command.CreatedAt,
                UpdatedAt: null,
                StartedAt: null,
                TimeSpent: null,
                Priority: command.Priority,
                State: null,
                AmountOfAgreements: null,
                TotalUpdates: 1),
            cancellationToken);

        await _analyticsRepository.AddDependencyAsync(
            new AddDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.DependsOnIds),
            cancellationToken);

        await _analyticsRepository.AddAssigneeAsync(
            new AddAssigneeQuery(
                Id: command.JobTaskId,
                AssigneeId: command.AssigneeId),
            cancellationToken);
    }

    public async Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken)
    {
        TaskAnalytics current = await _analyticsRepository.QueryAsync(
            new FetchAnalyticsQuery(command.JobTaskId),
            cancellationToken);

        await _analyticsRepository.UpsertAsync(
            new UpsertAnalyticsQuery(
                Id: command.JobTaskId,
                UpdatedAt: command.UpdatedAt,
                StartedAt: command.State.Equals(JobTaskState.InProgress) ? command.UpdatedAt : null,
                TimeSpent: command.State.Equals(JobTaskState.Done) ? command.UpdatedAt - current.StartedAt : null,
                CreatedAt: null,
                Priority: command.Priority > current.HighestPriority ? command.Priority : null,
                State: command.State,
                AmountOfAgreements: current.AmountOfAgreements + (command.IsAgreed.Equals(null) ? 0 : 1),
                TotalUpdates: current.TotalUpdates + 1),
            cancellationToken);
    }

    public async Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _analyticsRepository.AddDependencyAsync(
            new AddDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.Dependencies),
            cancellationToken);
    }

    public async Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _analyticsRepository.RemoveDependencyAsync(
            new RemoveDependencyQuery(
                Id: command.JobTaskId,
                Dependencies: command.Dependencies),
            cancellationToken);
    }
}
