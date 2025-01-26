using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Migrations;

#pragma warning disable SA1649

[Migration(1737842364, "enum job task state")]
public class EnumJobTaskState : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return
        """
        CREATE TYPE job_task_state AS ENUM (
            'none',
            'backlog',
            'to_do',
            'in_progress',
            'in_review',
            'done',
            'closed'
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return
        """
        DROP TYPE job_task_state;
        """;
    }
}
