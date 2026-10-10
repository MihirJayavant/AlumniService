# AlumniService Wiki

Welcome to the repository documentation. AlumniService groups alumni features into vertical slices and exposes them through HTTP, GraphQL and gRPC. These pages explain the current implementation, with source references and diagrams you can follow while making changes.

## Choose a starting point

| Your goal | Read next |
|---|---|
| Run the application | [Local Development](Local-Development.md) |
| Understand the system | [Architecture](Architecture.md) |
| Understand the tools and packages | [Technology Stack](Technology-Stack.md) |
| Add or modify a feature | [Patterns and Request Lifecycle](Patterns-and-Request-Lifecycle.md) |
| Understand records and validation | [Domain and Data Model](Domain-and-Data-Model.md) |
| Work with EF or migrations | [Persistence and Migrations](Persistence-and-Migrations.md) |
| Call the APIs | [API Transports](API-Transports.md) |
| Resume deferred auth work | [Authentication and Authorization Roadmap](Auth-Roadmap.md) |
| Understand generated properties | [Source Generation](Source-Generation.md) |
| Verify a change | [Testing and CI](Testing-and-CI.md) |

## Suggested reading order

Architecture → Technology Stack → Patterns and Request Lifecycle → Domain and Data Model → Persistence and Migrations. Read API Transports and Source Generation alongside the feature you are exploring.

## Documentation conventions

Pages use GitHub-flavored Markdown, relative file links and Mermaid diagrams. Each topic explains the behavior and points to its implementation. Diagrams distinguish runtime components, project references and compile-time generation; conceptual data diagrams do not replace the migration snapshot.

The documentation lives in `docs/` and renders directly in the repository. [`_Sidebar.md`](_Sidebar.md) and [`_Footer.md`](_Footer.md) provide wiki-style navigation; GitHub does not automatically turn this folder into its separate Wiki. Publishing there would require copying or syncing pages and adapting repository source links.

## Keep the docs current

Update the relevant page when changing runtime wiring, contracts, validation, persistence, package pins or build targets. Use source code as the authority when older guidance disagrees. Verify links and diagrams, use placeholders for credentials, and distinguish configured infrastructure from requirements actually applied to endpoints.

[Repository README](../README.md) · [Page navigation](_Sidebar.md)
