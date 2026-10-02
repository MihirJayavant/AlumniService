# AlumniService

An ASP.NET Core alumni API targeting .NET 10, organized into vertical slices for students, companies, exams, further studies, and faculty. Persistence uses Entity Framework Core with PostgreSQL; authentication uses ASP.NET Core Identity and JWT. The repository also includes a gRPC service and a separate YARP proxy.

## Repository layout

- `Source/Apps/AlumniBackendServices`: API endpoints, configuration, and EF Core migrations.
- `Source/Apps/AppHost`: Aspire orchestration for PostgreSQL, pgAdmin, and the API.
- `Source/Apps/ProxyApp`: standalone YARP proxy; not started by AppHost.
- `Source/Libraries`: domain features, shared types, infrastructure, and the record-view source generator.
- `Directory.Packages.props`: centrally managed NuGet versions.

## Prerequisites

- .NET SDK `10.0.401`, as pinned in `global.json`.
- A running Docker-compatible container runtime for Aspire's PostgreSQL and pgAdmin resources.
- A trusted development HTTPS certificate (`dotnet dev-certs https --trust`).

## Restore and build

Run from the repository root:

```sh
dotnet tool restore
dotnet restore AlumniService.slnx
dotnet build AlumniService.slnx
```

Alternatively, run the single-file Cake SDK build. `build.cs` uses `Cake.Sdk`, pinned in `global.json`; .NET restores the SDK automatically on first execution. The default target is `Build`, with `Release` configuration:

```sh
dotnet build.cs
dotnet build.cs -- --configuration=Debug --rebuild
```

`--rebuild` cleans the whole solution before building. Compiler warnings are reported using the SDK defaults.

GitHub Actions runs the same single command locally available for CI:

```sh
dotnet build.cs -- --target=CI
```

This restores the solution, verifies whitespace formatting against `.editorconfig`, checks code style and analyzer diagnostics at warning severity or higher, and builds in Release with warnings treated as errors. Checks fail without changing source files. Pass `--configuration=Debug` to use Debug. The workflow runs on pull requests, pushes to `main` (including merged pull requests), and manual dispatches.

## Run with Aspire

Configure local PostgreSQL credentials through the AppHost's user secrets:

```sh
dotnet user-secrets set "Parameters:pg-user" "alumni-service" --project Source/Apps/AppHost
dotnet user-secrets set "Parameters:pg-password" "<local-password>" --project Source/Apps/AppHost
dotnet run --project Source/Apps/AppHost
```

Open the dashboard URL printed by AppHost to find the API and pgAdmin endpoints. PostgreSQL uses a persistent data volume; changing credentials does not reset an existing database volume.

Aspire supplies `ConnectionStrings:alumni-db` to the API, including its allocated host port and credentials. The API prefers this complete connection string and passes it directly to EF Core.

## Run the API directly

Start an existing PostgreSQL database and configure a complete connection string using the environment variable `ConnectionStrings__alumni-db`. If absent, the API falls back to `Database:Connection`, replacing its `{0}` placeholder with `Database:Password` in development or `DATABASE_PASSWORD` otherwise.

Configure `Authentication:Secret`, `Authentication:ValidAudience`, and `Authentication:ValidIssuer` using local configuration or their double-underscore environment variable equivalents. Keep credentials and JWT secrets out of committed files.

```sh
dotnet run --project Source/Apps/AlumniBackendServices
```

## Codex agent workflow

Repository guidance lives in `AGENTS.md` and scoped files beneath `Source/`. Reusable workflows are in `.agents/skills`; project subagent roles are in `.codex/agents`. Keep independent workers within assigned file ownership and let the parent agent perform the final build.

Run the reusable Cake targets from the repository root:

```sh
dotnet build.cs -- --target=Doctor
dotnet build.cs -- --target=Bootstrap
dotnet build.cs
dotnet build.cs -- --configuration=Debug
dotnet build.cs -- --target=Run-Local
dotnet build.cs -- --target=Add-Migration --MigrationName=AddStudentField
```

`Doctor` fails for SDK/tool errors, warns about runtime prerequisites, and lists configuration requirements without reading secret values. If the EF tool is missing, run `Bootstrap` first. `Build` defaults to Release; pass `--configuration=Debug` for Debug builds. Migration generation requires API configuration and does not apply migrations.

Source-generator debugger launch is disabled by default. Set `ALUMNI_GENERATOR_DEBUG=1` only when intentionally debugging the generator in a Debug build. There is no dedicated test project; use CI checks and relevant manual runtime checks.

## Database migrations

Migrations live in the API project. Configure its database connection before running EF commands. For the Aspire database, use the connection string shown in the dashboard so the allocated port matches.

```sh
dotnet ef migrations add <MigrationName> --project Source/Apps/AlumniBackendServices --startup-project Source/Apps/AlumniBackendServices -- --environment Development
dotnet ef database update --project Source/Apps/AlumniBackendServices --startup-project Source/Apps/AlumniBackendServices -- --environment Development
```

Apply migrations before using endpoints that require database tables; AppHost does not apply them automatically.

Cake also supports creating a migration through the restored `dotnet-ef` tool:

```sh
dotnet build.cs -- --target=Add-Migration --MigrationName=AddStudentField
```
