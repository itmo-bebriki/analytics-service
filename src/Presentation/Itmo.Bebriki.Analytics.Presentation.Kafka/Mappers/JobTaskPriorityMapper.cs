using Itmo.Bebriki.Analytics.Application.Models.JobTask;

namespace Itmo.Bebriki.Analytics.Presentation.Kafka.Mappers;

internal static class JobTaskPriorityMapper
{
    internal static JobTaskPriority? ToInternal(
        Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority jobTaskPriority)
    {
        return jobTaskPriority switch
        {
            Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority.Low => JobTaskPriority.Low,
            Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority.Medium => JobTaskPriority.Medium,
            Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority.High => JobTaskPriority.High,
            Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority.Critical => JobTaskPriority.Critical,
            Itmo.Bebriki.Analytics.Kafka.Enums.JobTaskPriority.Unspecified => null,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskPriority), jobTaskPriority, null),
        };
    }
}
