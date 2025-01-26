using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Dev.Platform.Persistence.Abstractions.Transactions;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Itmo.Bebriki.Analytics.Application;

public class TransactionManager : ITransactionManager
{
    private readonly IPersistenceTransactionProvider _transactionProvider;
    private readonly ILogger<TransactionManager> _logger;

    public TransactionManager(
        IPersistenceTransactionProvider transactionProvider,
        ILogger<TransactionManager> logger)
    {
        _transactionProvider = transactionProvider;
        _logger = logger;
    }

    public async Task<T> RunAsync<T>(
        Func<Task<T>> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
    {
        _logger.LogInformation($"Starting transaction with {isolationLevel.ToString()}");

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            isolationLevel,
            cancellationToken);

        try
        {
            T result = await task.Invoke();
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RunAsync(
        Func<Task> task,
        CancellationToken cancellationToken,
        IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
    {
        _logger.LogInformation($"Starting transaction with {isolationLevel.ToString()}");

        await using IPersistenceTransaction transaction = await _transactionProvider.BeginTransactionAsync(
            isolationLevel,
            cancellationToken);

        try
        {
            await task.Invoke();
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
