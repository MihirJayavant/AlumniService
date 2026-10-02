# Local Orchestration

AppHost starts PostgreSQL with pgAdmin and a persistent volume, then the API. It passes the `alumni-db` connection string and waits for PostgreSQL. It does not start ProxyApp or apply migrations.

Keep `pg-user` and `pg-password` as secret parameters configured through user secrets. Do not print secret values or persist them in tracked configuration. Credential changes do not reset an existing PostgreSQL volume; investigate before proposing data deletion.

Use `dotnet build.cs -- --target=Doctor` for prerequisite diagnostics and `dotnet build.cs -- --target=Run-Local` to start Aspire. Ports are allocated by Aspire; obtain the current endpoint from the dashboard rather than hard-coding a database port. Preserve existing resource names unless the task requires a coordinated configuration change.
