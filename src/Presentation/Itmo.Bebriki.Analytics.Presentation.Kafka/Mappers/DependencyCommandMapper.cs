using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Tasks.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class DependencyCommandMapper
{
    public static DependencyCommand ToCommand(
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message)
    {
        if (message.Value.EventCase == JobTaskInfoValue.EventOneofCase.JobTaskDependenciesAdded)
        {
            return new DependencyCommand(
                JobTaskId: message.Key.JobTaskId,
                Dependencies: message.Value.JobTaskDependenciesAdded.AddedDependencies.ToArray(),
                UpdatedAt: message.Value.JobTaskDependenciesAdded.UpdatedAt.ToDateTimeOffset());
        }

        return new DependencyCommand(
            JobTaskId: message.Key.JobTaskId,
            Dependencies: message.Value.JobTaskDependenciesRemoved.RemovedDependencies.ToArray(),
            UpdatedAt: message.Value.JobTaskDependenciesRemoved.UpdatedAt.ToDateTimeOffset());
    }
}
