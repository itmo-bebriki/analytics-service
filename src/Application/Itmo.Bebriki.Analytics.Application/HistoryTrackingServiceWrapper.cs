using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Itmo.Bebriki.Analytics.Application;

public class HistoryTrackingServiceWrapper : IAnalyticsService
{
    private readonly IPersistenceContext _context;
    private readonly IAnalyticsService _wrappee;
    private readonly IPersistenceTransactionProvider _transactionProvider;

    public HistoryTrackingServiceWrapper(
        IServiceProvider provider,
        IPersistenceContext context,
        IPersistenceTransactionProvider transactionProvider)
    {
        _wrappee = provider.GetRequiredService<AnalyticsService>();
        _context = context;
        _transactionProvider = transactionProvider;
    }

    public async Task<TaskAnalytics?> GetAnalyticsByIdAsync(long id, CancellationToken cancellationToken)
    {
        return await _wrappee.GetAnalyticsByIdAsync(id, cancellationToken);
    }

    public async Task<PagedHistoryEvents> GetHistoryByIdAsync(FetchHistoryCommand command, CancellationToken cancellationToken)
    {
        return await _wrappee.GetHistoryByIdAsync(command, cancellationToken);
    }

    public async Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _wrappee.ProcessCreationAsync(command, cancellationToken);

        await _context.EventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.Creation,
                    Timestamp: command.CreatedAt,
                    Command: command)),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _wrappee.ProcessUpdateAsync(command, cancellationToken);

        await _context.EventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.Update,
                    Timestamp: command.UpdatedAt,
                    Command: command)),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _wrappee.ProcessNewDependencyAsync(command, cancellationToken);

        // TODO: put timestamp to proto contract
        await _context.EventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.NewDependency,
                    Timestamp: DateTimeOffset.UtcNow,
                    Command: command)),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);

        await _wrappee.ProcessPruneDependencyAsync(command, cancellationToken);

        // TODO: put timestamp to proto contract
        await _context.EventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.PruneDependency,
                    Timestamp: DateTimeOffset.UtcNow,
                    Command: command)),
            cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}
