using Itmo.Bebriki.Analytics.Application.Contracts.Commands;
using Itmo.Bebriki.Analytics.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class CreateJobTaskMapper
{
    public static CreateJobTaskCommand ToCommand(
        IKafkaConsumerMessage<JobTaskInfoKey, JobTaskInfoValue> message)
    {
        return new CreateJobTaskCommand(
            JobTaskId: message.Key.JobTaskId,
            Title: message.Value.JobTaskCreated.Title,
            Description: message.Value.JobTaskCreated.Description,
            AssigneeId: message.Value.JobTaskCreated.AssigneeId,
            Priority: JobTaskPriorityMapper.ToInternal(message.Value.JobTaskCreated.Priority),
            DependsOnIds: message.Value.JobTaskCreated.DependOnTaskIds.ToArray(),
            CreatedAt: message.Value.JobTaskCreated.CreatedAt.ToDateTimeOffset(),
            Deadline: message.Value.JobTaskCreated.DeadLine.ToDateTimeOffset());
    }
}
