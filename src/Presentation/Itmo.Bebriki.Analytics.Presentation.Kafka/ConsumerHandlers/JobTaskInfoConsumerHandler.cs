using Itmo.Bebriki.Analytics.Application.Contracts;
using Itmo.Bebriki.Analytics.Kafka.Contracts;
using Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;
using Itmo.Dev.Platform.Kafka.Consumer;
using Microsoft.Extensions.Logging;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.ConsumerHandlers;

public class JobTaskInfoConsumerHandler
    : IKafkaInboxHandler<JobTaskInfoKey, JobTaskInfoValue>
{
    private readonly ILogger<JobTaskInfoConsumerHandler> _logger;
    private readonly IAnalyticsService _service;

    public JobTaskInfoConsumerHandler(
        IAnalyticsService service,
        ILogger<JobTaskInfoConsumerHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async ValueTask HandleAsync(
        IEnumerable<IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue>> messages,
        CancellationToken cancellationToken)
    {
        foreach (IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message in messages)
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
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating job task {message.Value.JobTaskCreated.JobTaskId}");

        await _service.ProcessCreationAsync(
            CreateJobTaskMapper.ToCommand(message),
            cancellationToken);
    }

    private async Task HandleUpdate(
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating job task {message.Value.JobTaskUpdated.JobTaskId}");

        await _service.ProcessUpdateAsync(
            UpdateJobTaskMapper.ToCommand(message),
            cancellationToken);
    }

    private async Task HandleNewDependency(
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new dependency {message.Value.JobTaskDependenciesAdded.AddedDependencies} " +
                               $"to {message.Value.JobTaskDependenciesAdded.JobTaskId}");

        await _service.ProcessNewDependencyAsync(
            DependencyCommandMapper.ToCommand(message),
            cancellationToken);
    }

    private async Task HandlePruneDependency(
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding new dependency {message.Value.JobTaskDependenciesRemoved.RemovedDependencies} " +
                               $"to {message.Value.JobTaskDependenciesRemoved.JobTaskId}");

        await _service.ProcessNewDependencyAsync(
            DependencyCommandMapper.ToCommand(message),
            cancellationToken);
    }
}
