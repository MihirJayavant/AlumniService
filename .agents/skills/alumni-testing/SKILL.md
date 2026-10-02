---
name: alumni-testing
description: Plan, implement, or review AlumniService unit tests and test project organization; identify database-dependent cases for separate integration testing.
---

# Alumni testing

Read README.md, root AGENTS.md, and scoped instructions for the source and test areas. Paths below are relative to the repository root. Inspect the working tree and preserve unrelated changes.

## Define behavior before implementation

Inspect the public behavior and callers of the assigned component. Produce a compact scenario matrix: trigger/input, expected result, and unit or integration classification. Cover meaningful success, failure, and boundary cases rather than simple declarations or implementation details.

For Core, start with Email, HandlerExtensions.Execute, PaginatedList, and PaginationQuery.WithItems. Execute combines validation, invocation, OneOf results, exception handling, and cancellation; verify those contracts together. Flag unresolved semantics such as validation status, cancellation propagation, and invalid pagination input. Distinguish existing behavior from proposed changes. Resolve material contract decisions with the parent using the user's instructions; do not silently make production fixes or turn suspected defects into permanent expectations.

## Project conventions

For a new suite, prefer xUnit v3 with Microsoft.Testing.Platform, built-in assertions, and small handwritten doubles. This is a repository preference, subject to the user's choice. Verify current framework packages and runner setup against official documentation before scaffolding. Pin versions in Directory.Packages.props and inherit repository target framework and analyzer settings.

Place pure Core tests in Tests/Core.UnitTests, reference Core.csproj, and group files by behavior (for example EmailTests.cs, HandlerExtensionsTests.cs, and Pagination/). Use Fact for individual scenarios and Theory for input tables. Name tests Method_WhenCondition_ExpectedOutcome. Keep test data and state independent; await asynchronous operations and avoid timing-based cancellation assertions. Add shared helpers only when multiple tests need them.

PaginationQuery.Paginate uses EF Core asynchronous queries. Classify provider execution as integration testing, preferably in Tests/Core.IntegrationTests against PostgreSQL when requested. Plain AsQueryable lists cannot exercise this provider path; do not mock DbSet or introduce EF InMemory to claim PostgreSQL query coverage. RecordView generator behavior belongs in generator tests.

## Delegation and ownership

Use test-worker for independent groups of test files when delegation helps; keep small tasks in the parent. Assign a goal, exact owned files, agreed contracts, acceptance criteria, and required evidence. If the role is unavailable in the current session, use worker with the same instructions explicitly supplied.

The parent owns test project scaffolding, solution/package files, build.cs, CI configuration, and production changes unless assigning an exclusive owner. Workers must coordinate before editing outside their scope and accommodate others' changes. Never run concurrent restore, build, test, or EF commands in a shared checkout.

Have reviewer inspect meaningful assertions, missing branches, test independence, and accidental contract changes when an independent review helps. The parent integrates findings and performs final verification.

## Verification and delivery

For workflow-only setup, validate agent TOML and skill frontmatter and inspect the diff; no .NET build is needed. For test implementation, run dotnet build.cs and the configured test runner serially from the repository root. Verify discovery actually executes the intended tests; a build alone is not test evidence. Report the commands, executed/passed/failed counts, and blockers accurately.

When test execution wiring is in scope, add a Cake Test target and include test execution in CI after building. Check global.json and runner-specific command syntax first; do not assume VSTest and Microsoft.Testing.Platform accept the same options. Update README.md with verified commands once the suite exists.

Return changed files, covered scenarios, verification evidence, and unresolved contracts or deferred integration cases. Do not add test projects, test cases, packages, or CI wiring for a request limited to planning or agent/workflow setup.
