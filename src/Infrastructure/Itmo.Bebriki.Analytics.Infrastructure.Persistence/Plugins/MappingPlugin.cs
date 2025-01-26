using Itmo.Bebriki.Analytics.Application.Models.EventHistory;
using Itmo.Bebriki.Analytics.Application.Models.JobTask;
using Itmo.Dev.Platform.Persistence.Postgres.Plugins;
using Npgsql;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Plugins;

public class MappingPlugin : IPostgresDataSourcePlugin
{
    public void Configure(NpgsqlDataSourceBuilder dataSource)
    {
        dataSource.MapEnum<JobTaskState>("job_task_state");
        dataSource.MapEnum<JobTaskPriority>("job_task_priority");
        dataSource.MapEnum<EventType>("event_type");
    }
}
