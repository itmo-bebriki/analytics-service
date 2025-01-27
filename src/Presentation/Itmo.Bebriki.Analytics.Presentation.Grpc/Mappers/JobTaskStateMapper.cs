using Itmo.Bebriki.Analytics.Grpc.Enums;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Mappers;

internal static class JobTaskStateMapper
{
    internal static JobTaskState ToGrpc(Application.Models.JobTask.JobTaskState jobTaskState)
    {
        return jobTaskState switch {
            Application.Models.JobTask.JobTaskState.Unspecified => JobTaskState.Unspecified,
            Application.Models.JobTask.JobTaskState.PendingApproval => JobTaskState.PendingApproval,
            Application.Models.JobTask.JobTaskState.Approved => JobTaskState.Approved,
            Application.Models.JobTask.JobTaskState.Rejected => JobTaskState.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskState), jobTaskState, null),
        };
    }
}
