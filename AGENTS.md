# Repository Guidelines

## Project Structure & Modules

This is a .NET solution organized under `Source/`. `Source/Apps/AlumniBackendServices` contains the HTTP, GraphQL, and gRPC API, controllers, and EF Core migrations. `Source/Apps/ProxyApp` hosts the YARP proxy, while `Source/Apps/AppHost` configures the .NET Aspire development environment. Reusable code is in `Source/Libraries/`: domain slices such as `Alumni.Student` and `Alumni.Faculty`, plus `Core`, `Infrastructure`, and `Generators`. Keep features grouped by domain and their use cases rather than adding unrelated shared code to the API project. The repository currently has no dedicated test project.

## Build & Run

The required SDK is pinned in `global.json` (currently .NET SDK `10.0.401`). From the repository root:

- `dotnet restore AlumniService.slnx` restores solution dependencies.
- `dotnet build AlumniService.slnx` builds all projects.
- `dotnet run --project Source/Apps/AppHost` starts the Aspire app host and its configured services.
- `dotnet run --project Source/Apps/AlumniBackendServices` runs the API directly when its dependencies and configuration are available.

`build.cs` uses Cake.Sdk, pinned in `global.json`. Run `dotnet build.cs` to build, or `dotnet build.cs -- --configuration=Debug --rebuild` to clean and build. It also defines an `Add-Migration` target using the repository's .NET EF tool.

## Style & Naming

Follow the existing C# conventions: four-space indentation, braces on separate lines, PascalCase for types and public members, and descriptive domain-oriented names. Keep a feature's handlers, models, and persistence configuration near that feature (for example, `Alumni.Student/Company`). Match existing `.csproj` and namespace conventions. Pin dependency versions through `Directory.Packages.props` rather than adding scattered versions.

## Testing

No test suite or test project is currently present. Agent/workflow setup alone excludes test cases and test execution wiring. For explicitly requested test work, use `.agents/skills/alumni-testing/SKILL.md` and the `test-worker` role when delegation helps. Validate code changes with `dotnet build.cs`; once tests exist, also run the configured test runner. Use relevant manual runtime checks when dependencies are available. Report checks performed and any blocked checks accurately.

## Commits & Pull Requests

Recent history uses concise imperative summaries, sometimes with issue numbers (for example, `Add Aspire to project (#1)`). Follow that style. A pull request should explain the change and motivation, link its issue when applicable, note build or test results, and include screenshots for visible UI or API documentation changes.

## Configuration & Secrets

Use `appsettings.Development.json` or local secret storage for development-only settings. Never commit passwords, tokens, or production connection strings; review configuration changes for accidental secrets before opening a pull request.

## Agent Workflow

Read `README.md` for setup and the scoped `AGENTS.md` for the area being changed. When starting from the repository root, explicitly read the applicable scoped files before editing; do not assume they were automatically loaded.

- `build.cs` provides Build, Bootstrap, Doctor, Run-Local, and Add-Migration targets. Run commands from the repository root.
- Domain behavior belongs in `Source/Libraries/Alumni.Student` or `Alumni.Faculty`; transport wiring belongs in `Source/Apps/AlumniBackendServices`.
- Trace a feature from its endpoint through `IHandler.Execute`, validation, mapping, persistence, and HTTP result conversion before changing it.
- `[RecordView]` produces properties at compile time. Edit the source model or generator rather than generated files in `obj/`.
- GraphQL implementation files exist, but GraphQL wiring is currently disabled in `Program.cs`. ProxyApp runs separately from AppHost.
- Preserve unrelated local changes. Keep package versions in `Directory.Packages.props`.

Repository skills in `.agents/skills` cover feature changes, migrations, local debugging, and testing. Use the relevant skill for those workflows, without loading unrelated skills.

## Subagent Coordination

Use subagents for independent exploration, review, or implementation with disjoint ownership when the task benefits from delegation. Keep small or dependent tasks in the parent agent. Project role definitions are in `.codex/agents`.

Give each worker a goal, owned files/directories, acceptance criteria, and required evidence. The parent owns integration and final verification. Assign a single owner to shared files such as `Program.cs`, `ApplicationContext.cs`, solution/package files, and migrations/model snapshots. Workers must coordinate before editing outside their scope.

Avoid concurrent builds or EF commands in the same checkout because they share `bin/` and `obj/`. Use separate worktrees for overlapping implementations; do not let agents independently generate competing migrations. Summaries should name changed files, behavior, validation results, and remaining limitations.
