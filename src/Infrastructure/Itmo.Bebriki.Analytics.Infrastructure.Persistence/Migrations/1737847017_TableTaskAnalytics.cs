using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Migrations;

#pragma warning disable SA1649

[Migration(1737847017, "table task analytics")]
public class TableTaskAnalytics : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return
        """
        CREATE TABLE task_analytics (
            id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            created_at TIMESTAMP WITH TIME ZONE,
            last_update TIMESTAMP WITH TIME ZONE,
            started_at TIMESTAMP WITH TIME ZONE,
            time_spent TIMESTAMP WITH TIME ZONE,
            highest_priority job_task_priority DEFAULT 'none',
            current_state job_task_state DEFAULT 'none',
            amount_of_agreements INTEGER DEFAULT 0,
            total_updates INTEGER DEFAULT 0,
            assignees BIGINT[] DEFAULT ARRAY[]::BIGINT[],
            dependencies BIGINT[] DEFAULT ARRAY[]::BIGINT[]
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return
        """
        DROP TABLE task_analytics;
        """;
    }
}
