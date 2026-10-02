---
name: alumni-migration
description: Generate and review EF Core migrations for AlumniService PostgreSQL model changes.
---

# alumni-migration

Read Infrastructure and API scoped AGENTS.md files. Paths below are relative to the repository root.

Trace the model through its domain EF configuration and `Source/Libraries/Infrastructure/ApplicationContext.cs`. Verify context-interface and configuration registration changes. Migrations are owned by `Source/Apps/AlumniBackendServices`, not Infrastructure.

Coordinate a single migration owner. Run `dotnet build.cs -- --target=Add-Migration --MigrationName=<DescriptiveName>` using API development configuration from README.md. The local dotnet-ef version is pinned in `.config/dotnet-tools.json`; restore with `dotnet build.cs -- --target=Bootstrap` if needed.

Inspect the migration and model snapshot for intended tables, constraints, nullability, defaults, and potential data loss. Never hand-edit generated files just to conceal model drift. Build with `dotnet build.cs` after generation.

Database application is a separate operation requiring task authorization and an identified database. Do not reset Aspire volumes to resolve credential or migration errors. Use Aspire's allocated connection string, never an assumed host port. Do not print connection strings or credentials.

Report schema changes, data implications, generated files, and checks performed. If configuration blocks generation, explain the missing configuration keys without exposing values.
