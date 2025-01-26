using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

internal static class JobTaskPriorityMapper
{
    internal static JobTaskPriority? ToInternal(
        Itmo.Bebriki.Analytics.Kafka.Contracts.JobTaskPriority jobTaskPriority)
    {
        return jobTaskPriority switch
        {
            Analytics.Kafka.Contracts.JobTaskPriority.Low => JobTaskPriority.Low,
            Analytics.Kafka.Contracts.JobTaskPriority.Medium => JobTaskPriority.Medium,
            Analytics.Kafka.Contracts.JobTaskPriority.High => JobTaskPriority.High,
            Analytics.Kafka.Contracts.JobTaskPriority.Critical => JobTaskPriority.Critical,
            Analytics.Kafka.Contracts.JobTaskPriority.Unspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskPriority), jobTaskPriority, null),
        };
    }
}
