using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class JobTaskStateMapper
{
    public static JobTaskState? ToInternal(Analytics.Kafka.Enums.JobTaskState jobTaskState)
    {
        return jobTaskState switch
        {
            Analytics.Kafka.Enums.JobTaskState.Unspecified => JobTaskState.Unspecified,
            Analytics.Kafka.Enums.JobTaskState.PendingApproval => JobTaskState.PendingApproval,
            Analytics.Kafka.Enums.JobTaskState.Approved => JobTaskState.Approved,
            Analytics.Kafka.Enums.JobTaskState.Rejected => JobTaskState.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskState), jobTaskState, null),
        };
    }
}
