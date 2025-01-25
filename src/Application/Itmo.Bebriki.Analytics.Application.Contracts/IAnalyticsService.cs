using Itmo.Bebriki.Analytics.Application.Contracts.Commands;

namespace Itmo.Bebriki.Analytics.Application.Contracts;

public interface IAnalyticsService
{
    public Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken);

    public Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken);

    public Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken);

    public Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken);
}
