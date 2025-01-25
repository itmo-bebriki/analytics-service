using Itmo.Bebriki.Analytics.Application.Models;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

public static class JobTaskPriorityMapper
{
    public static JobTaskPriority ToInternal(
        Itmo.Bebriki.Analytics.Kafka.Contracts.JobTaskPriority jobTaskPriority)
    {
        return jobTaskPriority switch
        {
            Analytics.Kafka.Contracts.JobTaskPriority.Low => JobTaskPriority.Low,
            Analytics.Kafka.Contracts.JobTaskPriority.Medium => JobTaskPriority.Medium,
            Analytics.Kafka.Contracts.JobTaskPriority.High => JobTaskPriority.High,
            Analytics.Kafka.Contracts.JobTaskPriority.Critical => JobTaskPriority.Critical,
            Analytics.Kafka.Contracts.JobTaskPriority.Unspecified => throw new ArgumentOutOfRangeException(
                nameof(jobTaskPriority),
                jobTaskPriority,
                null),
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskPriority), jobTaskPriority, null),
        };
    }
}
