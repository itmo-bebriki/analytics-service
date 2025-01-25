using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class JobTaskStateMapper
{
    public static JobTaskState? ToInternal(Analytics.Kafka.Contracts.JobTaskState jobTaskState)
    {
        return jobTaskState switch
        {
            Analytics.Kafka.Contracts.JobTaskState.Backlog => JobTaskState.Backlog,
            Analytics.Kafka.Contracts.JobTaskState.ToDo => JobTaskState.ToDo,
            Analytics.Kafka.Contracts.JobTaskState.InProgress => JobTaskState.InProgress,
            Analytics.Kafka.Contracts.JobTaskState.InReview => JobTaskState.InReview,
            Analytics.Kafka.Contracts.JobTaskState.Done => JobTaskState.Done,
            Analytics.Kafka.Contracts.JobTaskState.Closed => JobTaskState.Closed,
            Analytics.Kafka.Contracts.JobTaskState.Unspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskState), jobTaskState, null),
        };
    }
}
