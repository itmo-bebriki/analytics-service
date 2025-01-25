using FluentMigrator;
using Itmo.Dev.Platform.Persistence.Postgres.Migrations;

namespace Itmo.Bebriki.Analytics.Infrastructure.Persistence.Migrations;

#pragma warning disable SA1649

[Migration(1737842874, "table event history")]
public class TableEventHistory : SqlMigration
{
    protected override string GetUpSql(IServiceProvider serviceProvider)
    {
        return
        """
        CREATE TABLE event_history (
            id BIGINT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            type event_type NOT NULL,
            occurred_at TIMESTAMP WITH TIME ZONE NOT NULL,
            payload JSONB NOT NULL
        );
        """;
    }

    protected override string GetDownSql(IServiceProvider serviceProvider)
    {
        return
        """
        DROP TABLE event_history;
        """;
    }
}
