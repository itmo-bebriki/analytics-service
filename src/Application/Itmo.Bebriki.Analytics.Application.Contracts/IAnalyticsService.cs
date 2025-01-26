using Itmo.Bebriki.Analytics.Application.Models.Analytics;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;

namespace Itmo.Bebriki.Analytics.Application.Contracts;

public interface IAnalyticsService
{
    public Task<TaskAnalytics?> GetAnalyticsByIdAsync(long id, CancellationToken cancellationToken);

    public Task<PagedHistoryEvents> GetHistoryByIdAsync(FetchHistoryCommand command, CancellationToken cancellationToken);

    public Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken);

    public Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken);

    public Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken);

    public Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken);
}
