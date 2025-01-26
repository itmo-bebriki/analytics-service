using Itmo.Bebriki.Analytics.Grpc.Contracts;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Mapper;

internal static class JobTaskPriorityMapper
{
    internal static JobTaskPriority ToGrpc(Application.Models.JobTask.JobTaskPriority jobTaskPriority)
    {
        return jobTaskPriority switch
        {
            Application.Models.JobTask.JobTaskPriority.None => JobTaskPriority.Unspecified,
            Application.Models.JobTask.JobTaskPriority.Low => JobTaskPriority.Low,
            Application.Models.JobTask.JobTaskPriority.Medium => JobTaskPriority.Medium,
            Application.Models.JobTask.JobTaskPriority.High => JobTaskPriority.High,
            Application.Models.JobTask.JobTaskPriority.Critical => JobTaskPriority.Critical,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskPriority), jobTaskPriority, null),
        };
    }
}
