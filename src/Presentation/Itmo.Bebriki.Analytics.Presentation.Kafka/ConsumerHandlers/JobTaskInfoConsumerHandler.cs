using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Kafka.Contracts;
using Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;
using Itmo.Dev.Platform.Kafka.Consumer;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.ConsumerHandlers;

public class JobTaskInfoConsumerHandler
    : IKafkaConsumerHandler<JobTaskInfoKey, JobTaskInfoValue>
{
    private readonly ILogger<JobTaskInfoConsumerHandler> _logger;
    private readonly IAnalyticsService _service;
    private readonly ITransactionManager _transactionManager;

    public JobTaskInfoConsumerHandler(
        IAnalyticsService service,
        ITransactionManager transactionManager,
        ILogger<JobTaskInfoConsumerHandler> logger)
    {
        _service = service;
        _transactionManager = transactionManager;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message in messages)
        {
            switch (message.Value.EventCase)
            {
                case JobTaskInfoValue.EventOneofCase.JobTaskCreated:
                    await HandleCreation(message, cancellationToken);
                    break;
                case JobTaskInfoValue.EventOneofCase.JobTaskUpdated:
                    await HandleUpdate(message, cancellationToken);
                    break;
                case JobTaskInfoValue.EventOneofCase.JobTaskDependenciesAdded:
                    await HandleNewDependency(message, cancellationToken);
                    break;
                case JobTaskInfoValue.EventOneofCase.JobTaskDependenciesRemoved:
                    await HandlePruneDependency(message, cancellationToken);
                    break;
                default:
                    _logger.LogError($"Received unknown message with type {message.Value.EventCase}");
                    break;
            }
        }
    }

    private async Task HandleCreation(
        IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating job task {message.Value.JobTaskCreated.JobTaskId}");

        await _transactionManager.RunAsync(
            async () => await _service.ProcessCreationAsync(
                CreateJobTaskMapper.ToCommand(message),
                cancellationToken),
            cancellationToken,
            isolationLevel: IsolationLevel.ReadUncommitted);
    }

    private async Task HandleUpdate(
        IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating job task {message.Value.JobTaskUpdated.JobTaskId}");

        await _transactionManager.RunAsync(
            async () => await _service.ProcessUpdateAsync(
                UpdateJobTaskMapper.ToCommand(message),
                cancellationToken),
            cancellationToken,
            isolationLevel: IsolationLevel.ReadUncommitted);
    }

    private async Task HandleNewDependency(
        IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new dependency {message.Value.JobTaskDependenciesAdded.AddedDependencies} " +
                               $"to {message.Value.JobTaskDependenciesAdded.JobTaskId}");

        await _transactionManager.RunAsync(
            async () => await _service.ProcessNewDependencyAsync(
                DependencyCommandMapper.ToCommand(message),
                cancellationToken),
            cancellationToken,
            isolationLevel: IsolationLevel.ReadUncommitted);
    }

    private async Task HandlePruneDependency(
        IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new dependency {message.Value.JobTaskDependenciesRemoved.RemovedDependencies} " +
                               $"to {message.Value.JobTaskDependenciesRemoved.JobTaskId}");

        await _transactionManager.RunAsync(
            async () => await _service.ProcessNewDependencyAsync(
                DependencyCommandMapper.ToCommand(message),
                cancellationToken),
            cancellationToken,
            isolationLevel: IsolationLevel.ReadUncommitted);
    }
}
