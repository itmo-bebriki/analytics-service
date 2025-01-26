using Itmo.Bebriki.Analytics.Grpc.Contracts;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Mapper;

internal static class JobTaskStateMapper
{
    internal static JobTaskState ToGrpc(Application.Models.JobTask.JobTaskState jobTaskState)
    {
        return jobTaskState switch {
            Application.Models.JobTask.JobTaskState.None => JobTaskState.Unspecified,
            Application.Models.JobTask.JobTaskState.Backlog => JobTaskState.Backlog,
            Application.Models.JobTask.JobTaskState.ToDo => JobTaskState.ToDo,
            Application.Models.JobTask.JobTaskState.InProgress => JobTaskState.InProgress,
            Application.Models.JobTask.JobTaskState.InReview => JobTaskState.InReview,
            Application.Models.JobTask.JobTaskState.Done => JobTaskState.Done,
            Application.Models.JobTask.JobTaskState.Closed => JobTaskState.Closed,
            _ => throw new ArgumentOutOfRangeException(nameof(jobTaskState), jobTaskState, null),
        };
    }
}
