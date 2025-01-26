using Google.Protobuf.WellKnownTypes;
using Itmo.Bebriki.Analytics.Application.Models.Commands;
using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Bebriki.Analytics.Grpc.Contracts;
using Itmo.Bebriki.Analytics.Presentation.Grpc.Mapper;
using EventType = Itmo.Bebriki.Analytics.Application.Models.EventHistory.EventType;

namespace Itmo.Bebriki.Analytics.Presentation.Grpc.Mappers;

internal static class PayloadMapper
{
    internal static Payload FromInternal(PayloadEvent payloadEvent)
    {
        var tempPayload = new Payload
        {
            Id = payloadEvent.Id,
            EventType = EventTypeMapper.FromInternal(payloadEvent.EventType),
            Timestamp = payloadEvent.Timestamp.ToTimestamp(),
        };

        switch (payloadEvent.EventType)
        {
            case EventType.Creation:
                var creationCmd = (CreateJobTaskCommand)payloadEvent.Command;

                tempPayload.Create = new Payload.Types.CreatePayload
                {
                    JobTaskId = creationCmd.JobTaskId,
                    Title = creationCmd.Title,
                    Description = creationCmd.Description,
                    AssigneeId = creationCmd.AssigneeId,
                    CreatedAt = creationCmd.CreatedAt.ToTimestamp(),
                    Deadline = creationCmd.Deadline.ToTimestamp(),
                    Priority = JobTaskPriorityMapper.ToGrpc(creationCmd.Priority),
                    DependsOnIds = { creationCmd.DependsOnIds.ToArray() },
                };
                break;
            case EventType.Update:
                var updateCmd = (UpdateJobTaskCommand)payloadEvent.Command;

                tempPayload.Update = new Payload.Types.UpdatePayload
                {
                    JobTaskId = updateCmd.JobTaskId,
                    Title = updateCmd.Title,
                    Description = updateCmd.Description,
                    AssigneeId = updateCmd.AssigneeId,
                    State = updateCmd.State is null ? JobTaskState.Unspecified : JobTaskStateMapper.ToGrpc(updateCmd.State.Value),
                    Priority = updateCmd.Priority is null ? JobTaskPriority.Unspecified : JobTaskPriorityMapper.ToGrpc(updateCmd.Priority.Value),
                    DeadLine = updateCmd.Deadline?.ToTimestamp(),
                    UpdatedAt = updateCmd.UpdatedAt.ToTimestamp(),
                    IsAgreed = updateCmd.IsAgreed,
                };
                break;
            case EventType.NewDependency:
            case EventType.PruneDependency:
                var depsCmd = (DependencyCommand)payloadEvent.Command;

                tempPayload.Dependency = new Payload.Types.DependencyPayload
                {
                    JobTaskId = depsCmd.JobTaskId,
                    ChangedDependencies = { depsCmd.Dependencies.ToArray() },
                };
                break;
        }

        return tempPayload;
    }
}
