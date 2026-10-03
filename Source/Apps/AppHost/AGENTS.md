# Local Orchestration

AppHost starts PostgreSQL with pgAdmin and a persistent volume, then the API. It passes the `alumni-db` connection string and waits for PostgreSQL. It does not start ProxyApp or apply migrations. Run EF commands manually from the repository root with the development database connection configured.

Keep `pg-user` and `pg-password` as secret parameters configured through user secrets. Do not print secret values or persist them in tracked configuration. Credential changes do not reset an existing PostgreSQL volume; investigate before proposing data deletion.

Use `dotnet build.cs -- --target=Doctor` for prerequisite diagnostics and `dotnet build.cs -- --target=Run-Local` to start Aspire. PostgreSQL uses the fixed host port 5432; other service ports are allocated by Aspire. Obtain the complete database connection string from the dashboard when running EF commands outside AppHost. Preserve existing resource names unless the task requires a coordinated configuration change.
