using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;
using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Contracts.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory.Events;
using System.Text.Json;

namespace Itmo.Bebriki.Analytics.Application;

public class HistoryTrackingServiceWrapper : IAnalyticsService
{
    private readonly IAnalyticsService _wrappee;
    private readonly IEventHistoryRepository _eventHistoryRepository;

    public HistoryTrackingServiceWrapper(
        IAnalyticsService wrappee,
        IEventHistoryRepository eventHistoryRepository)
    {
        _wrappee = wrappee;
        _eventHistoryRepository = eventHistoryRepository;
    }

    public async Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await _wrappee.ProcessCreationAsync(command, cancellationToken);

        await _eventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.Creation,
                    Timestamp: command.CreatedAt,
                    Payload: JsonSerializer.Serialize(command))),
            cancellationToken);
    }

    public async Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken)
    {
        await _wrappee.ProcessUpdateAsync(command, cancellationToken);

        await _eventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.Update,
                    Timestamp: command.UpdatedAt,
                    Payload: JsonSerializer.Serialize(command))),
            cancellationToken);
    }

    public async Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _wrappee.ProcessNewDependencyAsync(command, cancellationToken);

        // TODO: put timestamp to proto contract
        await _eventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.NewDependency,
                    Timestamp: DateTimeOffset.Now,
                    Payload: JsonSerializer.Serialize(command))),
            cancellationToken);
    }

    public async Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        await _wrappee.ProcessPruneDependencyAsync(command, cancellationToken);

        // TODO: put timestamp to proto contract
        await _eventHistoryRepository.AddEventAsync(
            new AddEventQuery(
                new PayloadEvent(
                    Id: command.JobTaskId,
                    EventType: EventType.PruneDependency,
                    Timestamp: DateTimeOffset.Now,
                    Payload: JsonSerializer.Serialize(command))),
            cancellationToken);
    }
}
