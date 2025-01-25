using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Application.Contracts.Commands;

namespace Itmo.Bebriki.Analytics.Application;

public class AnalyticsService : IAnalyticsService
{
    public Task ProcessCreationAsync(CreateJobTaskCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessUpdateAsync(UpdateJobTaskCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessNewDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task ProcessPruneDependencyAsync(DependencyCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
