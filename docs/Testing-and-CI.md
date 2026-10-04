# Testing and CI

[Home](Home.md) · [Architecture](Architecture.md) · [README](../README.md)

## Test coverage

API tests cover successful and error HTTP result conversion, all 13 minimal API route registrations, the paginated FurtherStudy response metadata, strict JSON number handling, and configuration precedence/defaults. They inspect routes without starting a server and require no database, Docker, or secrets. Environment-password cases restore process environment values and run without parallel execution. Request binding, actual HTTP serialization, middleware, and persistence need separate integration tests.

Core tests use xUnit v3 and Microsoft.Testing.Platform, selected in `global.json`. They cover email validation and value behavior, handler validation/results/exceptions/cancellation, pagination calculations, and item mapping. They require no database, Docker, API configuration, or secrets.

Generator tests use the same test framework and run Roslyn against small C# inputs, verifying generated properties and compilation diagnostics. They cover required init properties, exclusions, member selection, nullable and generic types, namespaces, missing attributes, and multiple views. They require no database or API startup.

Faculty tests cover required fields, email syntax and normalized length limits, name and extension boundaries, positive mobile numbers, nonempty identifiers, pagination limits and offset overflow, and response mapping. Invalid requests are also executed through the real handlers to verify `BadRequest` without database access. These tests require no database, Docker, API configuration, or secrets. Persistence normalization, duplicate detection, successful CRUD, and actual page contents require separate PostgreSQL integration tests.

Student tests cover required fields and trimmed database length limits, email and mobile-number validation, branch lengths and supported gender values, birth dates and year chronology, nested addresses, lookup identifiers, pagination limits and offset overflow, and Company, Exam, and FurtherStudy validation. Invalid requests execute through real handlers to verify `BadRequest` without database access. Mapping tests cover creation identity, Student audit timestamps, text trimming, email normalization, canonical gender values, and every response mapper. These tests require no database, Docker, API configuration, or secrets. Successful persistence, duplicate detection, relationships, and actual page contents need separate PostgreSQL integration tests.

Build the solution and run its tests through Cake:

```sh
dotnet build.cs -- --target=Test
```

Run only the Core suite directly (builds and restores as needed):

```sh
dotnet test --project Tests/Core.UnitTests/Core.UnitTests.csproj --configuration Release
```

Build and run only the generator suite without build servers:

```sh
dotnet build Tests/Generators.UnitTests/Generators.UnitTests.csproj --configuration Release --disable-build-servers -m:1
dotnet Tests/Generators.UnitTests/bin/Release/net10.0/Generators.UnitTests.dll
```

Build and run only the Student suite without build servers:

```sh
dotnet build Tests/Alumni.Student.UnitTests/Alumni.Student.UnitTests.csproj --configuration Release --disable-build-servers -m:1
dotnet Tests/Alumni.Student.UnitTests/bin/Release/net10.0/Alumni.Student.UnitTests.dll
```

After a Release build, run all solution tests without rebuilding:

```sh
dotnet test --solution AlumniService.slnx --configuration Release --no-build --no-restore
```

`HandlerExtensions.Execute` returns `BadRequest` for validation failures, preserves handler-returned errors, propagates cancellation, and converts other exceptions to `InternalError`. EF-backed `PaginationQuery.Paginate` needs separate PostgreSQL integration tests; those are deferred. Pagination input validation has dedicated Core and domain tests.

## GraphQL coverage

The API suite also builds and inspects the GraphQL schema, checks field names and scalar types, verifies handler error codes and invalid inputs, tests cancellation propagation, and checks endpoint mapping. These isolated tests use a rejecting context where appropriate and do not establish successful database persistence.

## CI pipeline

The [GitHub Actions workflow](../.github/workflows/ci.yml) runs on pull requests, pushes to `main`, and manual dispatch. It selects the SDK from `global.json` and invokes:

```sh
dotnet build.cs -- --target=CI
```

The Cake dependency chain restores packages, builds the generator in Debug for the formatter workspace, verifies formatting and warning-level diagnostics without modifying files, builds with warnings treated as errors, and runs tests. Default configuration is Release.

## Integration boundaries

Unit tests do not replace checks of PostgreSQL queries, migrations, constraints, relationships, successful CRUD, or actual pagination contents. HTTP request binding, serialization and middleware also need runtime integration checks. Run database-dependent checks separately with deliberate local configuration.

## Contribution workflow

Read the root and scoped `AGENTS.md` files before changes. Repository skills live in `.agents/skills`, with subagent role definitions in `.codex/agents`. Assign disjoint file ownership and serialize restore/build/test and EF commands in a shared checkout. Never edit generated files under `obj/`.

For feature work, follow [Patterns and Request Lifecycle](Patterns-and-Request-Lifecycle.md). For new unit tests, use the repository's `alumni-testing` skill. Run the relevant checks and report blocked checks accurately.

## Source references

- [Core tests](../Tests/Core.UnitTests)
- [Generator tests](../Tests/Generators.UnitTests)
- [Student tests](../Tests/Alumni.Student.UnitTests)
- [Faculty tests](../Tests/Alumni.Faculty.UnitTests)
- [API and GraphQL tests](../Tests/Alumni.Api.UnitTests)
- [Build targets](../build.cs)
