using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Kafka.Contracts;
using Itmo.Dev.Platform.Kafka.Consumer;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class UpdateJobTaskMapper
{
    public static UpdateJobTaskCommand ToCommand(
        IKafkaInboxMessage<JobTaskInfoKey, JobTaskInfoValue> message)
    {
        return new UpdateJobTaskCommand(
            JobTaskId: message.Key.JobTaskId,
            Title: message.Value.JobTaskUpdated.Title,
            Description: message.Value.JobTaskUpdated.Description,
            AssigneeId: message.Value.JobTaskUpdated.AssigneeId,
            State: JobTaskStateMapper.ToInternal(message.Value.JobTaskUpdated.State),
            Priority: JobTaskPriorityMapper.ToInternal(message.Value.JobTaskUpdated.Priority),
            Deadline: message.Value.JobTaskUpdated.DeadLine?.ToDateTimeOffset() == DateTimeOffset.FromUnixTimeSeconds(0) ? null : message.Value.JobTaskUpdated.DeadLine?.ToDateTimeOffset(),
            IsAgreed: message.Value.JobTaskUpdated.IsAgreed,
            UpdatedAt: message.Value.JobTaskUpdated.UpdatedAt.ToDateTimeOffset());
    }
}
