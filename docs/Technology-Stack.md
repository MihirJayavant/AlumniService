# Technology Stack

[Home](Home.md) · [Architecture](Architecture.md) · [Local development](Local-Development.md)

The repository pins its SDK in [global.json](../global.json), shares compiler settings through [Directory.Build.props](../Directory.Build.props), and centrally manages NuGet versions in [Directory.Packages.props](../Directory.Packages.props). These files are the version authority when upgrading dependencies.

## Runtime and application code

| Technology | Repository use | Source |
| --- | --- | --- |
| .NET SDK 10.0.401 / C# | Application and test projects target `net10.0`; nullable references, required properties, records and extension blocks are used | [Shared build settings](../Directory.Build.props) |
| ASP.NET Core | Hosts minimal HTTP endpoints, middleware and transport integrations | [API startup](../Source/Apps/Alumni.Api/Program.cs) |
| FluentValidation | Each handler exposes a request validator; reusable rules live in Core | [Handler execution](../Source/Libraries/Core/Handler.cs), [validation helpers](../Source/Libraries/Core/ValidationExtensions.cs) |
| OneOf | Represents success or `ErrorType` as `OneOf<TResponse, ErrorType>` | [Handler contract](../Source/Libraries/Core/Handler.cs) |
| System.Text.Json | HTTP JSON serialization with strict number handling | [OpenAPI registration](../Source/Apps/Alumni.Api/ExtensionService/OpenApiExtension.cs) |

## Data and security

| Technology | Repository use | Source |
| --- | --- | --- |
| EF Core 10 | LINQ queries, change tracking, relationship configuration and migrations | [ApplicationContext](../Source/Libraries/Infrastructure/ApplicationContext.cs) |
| Npgsql EF provider | PostgreSQL database access; migration assembly is `Alumni.Api` | [Infrastructure services](../Source/Libraries/Infrastructure/ConfigureServices.cs) |
| ASP.NET Core Identity | User and role persistence in the shared DbContext | [ApplicationUser](../Source/Libraries/Infrastructure/Identity/ApplicationUser.cs) |
| JWT bearer authentication | Signing-key, issuer, audience and lifetime validation; role policies are registered | [Infrastructure services](../Source/Libraries/Infrastructure/ConfigureServices.cs) |
| ASP.NET Core health checks | `/healthz` includes a PostgreSQL check and a JSON health response | [Web API registration](../Source/Apps/Alumni.Api/ExtensionService/WebApiExtension.cs) |

Authentication registration and role policies do not imply that every endpoint requires authorization. Read the individual transport declarations when checking access requirements.

## API and development tooling

| Technology | Repository use | Source |
| --- | --- | --- |
| HotChocolate 16 | Active GraphQL queries and mutations at `/graphql`; resolver-scoped dependencies | [GraphQL registration](../Source/Apps/Alumni.Api/ExtensionService/GraphQLExtension.cs) |
| gRPC / Protocol Buffers | Five unary service adapters; server code generated from `alumni.v1` proto contracts | [gRPC registration](../Source/Apps/Alumni.Api/ExtensionService/GrpcExtension.cs), [proto contracts](../Source/Apps/Alumni.Api/Grpc/Protos/alumni/v1) |
| ASP.NET Core OpenAPI | Builds the HTTP OpenAPI document | [OpenAPI registration](../Source/Apps/Alumni.Api/ExtensionService/OpenApiExtension.cs) |
| Swagger UI and Scalar | Interactive HTTP API documentation in Development | [OpenAPI registration](../Source/Apps/Alumni.Api/ExtensionService/OpenApiExtension.cs) |
| Aspire 13.6 | File-based local orchestration for PostgreSQL, pgAdmin and API | [apphost.cs](../apphost.cs) |
| YARP 2.3 | Separate reverse proxy using the `ReverseProxy` configuration section | [Proxy startup](../Source/Apps/ProxyApp/Program.cs) |
| Roslyn incremental generator | Generates record-view properties at compile time; generator targets `netstandard2.0` | [Generator](../Source/Libraries/Generators/RecordViewGenerator.cs) |
| Cake.Sdk 6.3 | File-based build, local diagnostics, migration and CI targets | [build.cs](../build.cs) |
| xUnit v3 / Microsoft.Testing.Platform | Unit tests and generator compilation tests | [Testing and CI](Testing-and-CI.md) |
| GitHub Actions | Repository CI workflow | [Workflow directory](../.github/workflows) |

## Configuration ownership

- Package versions belong in `Directory.Packages.props`; project files select package references.
- Database and authentication settings are resolved by [SettingService](../Source/Apps/Alumni.Api/Services/SettingService.cs).
- Local passwords and JWT secrets belong in user secrets or local environment configuration.
- AppHost parameters and API settings have distinct configuration scopes. Aspire passes a complete database connection string to the API.

See [local development](Local-Development.md) for commands, [API transports](API-Transports.md) for client contracts, and [source generation](Source-Generation.md) for compile-time behavior.
