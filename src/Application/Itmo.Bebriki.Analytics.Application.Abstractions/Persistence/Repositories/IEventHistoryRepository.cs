using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory.Events;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;

public interface IEventHistoryRepository
{
    public IAsyncEnumerable<BaseEvent> QueryAsync(FetchQuery ctx, CancellationToken cancellationToken);

    public Task AddEventAsync(AddEventQuery ctx, CancellationToken cancellationToken);
}
