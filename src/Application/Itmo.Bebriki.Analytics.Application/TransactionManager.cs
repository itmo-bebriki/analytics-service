using Itmo.Bebriki.Analytics.Application.Contracts;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Itmo.Bebriki.Analytics.Application;

public class TransactionManager : ITransactionManager
{
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(ILogger<TransactionManager> logger)
    {
        _logger = logger;
    }

    public async Task<T> RunAsync<T>(
        Func<Task<T>> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
    {
        _logger.LogInformation($"Starting transaction with {isolationLevel.ToString()}");

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = isolationLevel },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            T result = await task.Invoke();
            transaction.Complete();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed");
            throw;
        }
    }

    public async Task RunAsync(
        Func<Task> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
    {
        _logger.LogInformation($"Starting transaction with {isolationLevel.ToString()}");

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = isolationLevel },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            await task.Invoke();
            transaction.Complete();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed");
            throw;
        }
    }
}
