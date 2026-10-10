# Local Development

[Home](Home.md) · [Architecture](Architecture.md) · [README](../README.md)

Run all commands from the repository root. The active Aspire host is the file-based [`apphost.cs`](../apphost.cs); the solution has no AppHost project.

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

`--rebuild` cleans the whole solution before building. Shared build settings enable analyzers and treat warnings as errors.

GitHub Actions runs the same single command locally available for CI:

```sh
dotnet build.cs -- --target=CI
```

This restores the solution, builds the record-view generator in Debug for the formatter's default workspace, runs the `Format` target to verify whitespace formatting against `.editorconfig` and check code style and analyzer diagnostics at warning severity or higher, builds in Release with warnings treated as errors, and runs the tests. Checks fail without changing source files. Pass `--configuration=Debug` to use Debug. The workflow runs on pull requests, pushes to `main` (including merged pull requests), and manual dispatches.

## Run with Aspire

Configure local PostgreSQL credentials through the file-based AppHost's user secrets:

```sh
dotnet user-secrets set "Parameters:pg-user" "alumni-service" --file apphost.cs
dotnet user-secrets set "Parameters:pg-password" "<local-password>" --file apphost.cs
dotnet run --file apphost.cs
```

The dashboard uses the fixed address `https://localhost:18888`, configured in `apphost.run.json`. Open the login URL printed by AppHost to find the API and pgAdmin endpoints. Telemetry and the AppHost resource service use separate HTTPS ports `18889` and `18891`; these ports must be available. PostgreSQL uses a persistent data volume; changing credentials does not reset an existing database volume.

PostgreSQL uses the fixed host port `5432`; this port must be available when starting AppHost. Aspire supplies `ConnectionStrings:alumni-db` to the API, including the host port and credentials. The API prefers this complete connection string and passes it directly to EF Core.

## Run the API directly

Start an existing PostgreSQL database and configure a complete connection string using the environment variable `ConnectionStrings__alumni-db`. If absent, the API falls back to `Database:Connection`, replacing its `{0}` placeholder with `Database:Password` in development or `DATABASE_PASSWORD` otherwise.

```sh
dotnet run --project Source/Apps/Alumni.Api
```

## Authentication configuration

The legacy Identity/JWT runtime setup has been removed. `Alumni.Auth` defines database storage only; login, invitations, token issuance and authorization enforcement will be added separately. Authentication configuration is not currently required for startup. Feature endpoints remain unprotected.

The host environment controls development-only API documentation. Separately, `SettingService` reads a configuration key named `Environment`, defaulting to `Development`, to select the fallback database password source. Do not assume `ASPNETCORE_ENVIRONMENT` alone selects that fallback. Prefer a complete `ConnectionStrings:alumni-db` value.

## Health checks

`GET /healthz` includes a PostgreSQL connectivity check. It can report unhealthy when the database is unavailable; it does not verify that migrations have been applied.

## Build helper targets

| Target | Purpose |
|---|---|
| `Bootstrap` | Restore local tools and solution packages |
| `Doctor` | Check SDK and EF tool, warn about Docker and HTTPS prerequisites |
| `Build` | Build the solution; default target and Release configuration |
| `Test` | Build, then run solution tests |
| `Format` | Verify formatting, code style and analyzer diagnostics |
| `CI` | Restore, format, build with warnings as errors, then test |
| `Add-Migration` | Generate a migration; does not apply it |

```sh
dotnet build.cs -- --target=Doctor
dotnet build.cs -- --target=Bootstrap
dotnet build.cs -- --configuration=Debug
dotnet build.cs -- --target=Test
```

There is no `Run-Local` target in the current build script. Use `dotnet run --file apphost.cs`.

## Troubleshooting

| Symptom | Check |
|---|---|
| SDK selection fails | Install the exact SDK from `global.json` |
| Aspire cannot start containers | Start the Docker-compatible runtime |
| Resource ports are occupied | Check ports 5432, 18888, 18889 and 18891 |
| Database rejects new credentials | An existing persistent volume retains its initialized credentials |
| Endpoint fails on missing tables | Apply migrations as described in [Persistence and Migrations](Persistence-and-Migrations.md) |

## Source references

- [Build targets](../build.cs)
- [Aspire resources](../apphost.cs) and [dashboard launch configuration](../apphost.run.json)
- [Configuration precedence](../Source/Apps/Alumni.Api/Services/SettingService.cs)
- [Infrastructure registration](../Source/Libraries/Infrastructure/ConfigureServices.cs)
