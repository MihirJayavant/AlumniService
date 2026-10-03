---
name: alumni-feature
description: Implement or modify AlumniService domain features across handlers, mappings, persistence, and API endpoints.
---

# alumni-feature

Trace the requested behavior from `Source/Apps/Alumni.Api/Controllers` into its owning domain. Read the root and applicable scoped AGENTS.md files.

Use `Source/Libraries/Alumni.Student/Company` as a slice reference, `Alumni.Student/AddStudentHandler.cs` for handler shape, `Core/Handler.cs` for execution semantics, and API `Controllers/ResultHelper.cs` for HTTP mapping. These paths are relative to the repository root, not this skill directory.

Keep changes in the owning feature. Follow the actual endpoint registration in `Controllers/Endpoint.cs`. Check validation, error status, cancellation, and manual mappers together. `[RecordView]` contracts obtain properties from source models during compilation; do not edit generated files. GraphQL wiring is currently disabled.

If persistence changes, use alumni-migration. Build with `dotnet build.cs`. Perform relevant manual runtime checks when configured; report unavailable dependencies. Do not introduce CI or test cases as part of the current agent-readiness scope.

Return changed behavior, affected files, build/runtime evidence, and remaining limitations.
