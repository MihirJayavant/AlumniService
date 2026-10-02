---
name: alumni-local-debug
description: Diagnose AlumniService local SDK, Aspire, PostgreSQL, authentication, and API startup problems.
---

# alumni-local-debug

Run `dotnet build.cs -- --target=Doctor` and read README.md plus scoped instructions for the affected app. Commands and source paths are relative to the repository root.

Separate build failures from runtime configuration failures. Use `dotnet build.cs -- --target=Bootstrap` for missing local tools/dependencies and `dotnet build.cs` for compilation. `Doctor` checks SDK/tool availability but does not verify secret values. Its Docker/certificate warnings describe runtime prerequisites.

AppHost requires secret parameters `Parameters:pg-user` and `Parameters:pg-password`. API authentication requires Secret, ValidAudience, and ValidIssuer under Authentication. Check key names and configuration sources without printing values. The Aspire `ConnectionStrings:alumni-db` overrides legacy database settings; see API `Services/SettingService.cs`.

Start with `dotnet build.cs -- --target=Run-Local` when runtime checks are needed. Use dashboard endpoints; PostgreSQL ports are allocated. `/healthz` includes a database check. AppHost neither applies migrations nor starts ProxyApp. GraphQL wiring is disabled.

Existing volumes retain credentials/data. Do not delete them as a routine repair. Keep source-generator debugging opt-in; `ALUMNI_GENERATOR_DEBUG=1` can launch a debugger during Debug builds.

Report the observed failure, owning component, concrete correction, and build/runtime evidence. Do not claim startup success based only on compilation.
