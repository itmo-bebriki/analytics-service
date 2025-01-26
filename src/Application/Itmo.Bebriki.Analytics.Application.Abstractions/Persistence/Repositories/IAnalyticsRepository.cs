using Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Queries;
using Itmo.Bebriki.Analytics.Application.Models.Analytics;

namespace Itmo.Bebriki.Analytics.Application.Abstractions.Persistence.Repositories;

public interface IAnalyticsRepository
{
    public Task<TaskAnalytics> QueryAsync(FetchAnalyticsQuery ctx, CancellationToken cancellationToken);

    public Task UpsertAsync(UpsertAnalyticsQuery ctx, CancellationToken cancellationToken);

    public Task AddDependencyAsync(AddDependencyQuery ctx, CancellationToken cancellationToken);

    public Task RemoveDependencyAsync(RemoveDependencyQuery ctx, CancellationToken cancellationToken);

    public Task AddAssigneeAsync(AddAssigneeQuery ctx, CancellationToken cancellationToken);

    public Task RemoveAssigneeAsync(RemoveAssigneeQuery ctx, CancellationToken cancellationToken);
}
