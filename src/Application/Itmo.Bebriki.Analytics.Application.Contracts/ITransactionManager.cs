using System.Transactions;

namespace Itmo.Bebriki.Analytics.Application.Contracts;

public interface ITransactionManager
{
    public Task<T> RunAsync<T>(
        Func<Task<T>> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

    public Task RunAsync(
        Func<Task> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);
}
