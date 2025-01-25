using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Migrations;

#pragma warning disable SA1649

[Migration(1737842487, "enum job task priority")]
public class EnumJobTaskPriority : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return
        """
        CREATE TYPE job_task_priority AS ENUM (
            'none',
            'low',
            'medium',
            'high',
            'critical'
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return
        """
        DROP TYPE job_task_priority;
        """;
    }
}
