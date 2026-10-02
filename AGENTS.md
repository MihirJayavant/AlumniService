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

No test suite or test project is currently present. For changes, run `dotnet build AlumniService.slnx`; add focused automated tests in a dedicated test project when introducing testable behavior, and use the standard `*Tests.cs` naming pattern.

## Commits & Pull Requests

Recent history uses concise imperative summaries, sometimes with issue numbers (for example, `Add Aspire to project (#1)`). Follow that style. A pull request should explain the change and motivation, link its issue when applicable, note build or test results, and include screenshots for visible UI or API documentation changes.

## Configuration & Secrets

Use `appsettings.Development.json` or local secret storage for development-only settings. Never commit passwords, tokens, or production connection strings; review configuration changes for accidental secrets before opening a pull request.
