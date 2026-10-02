# API Integration

Classes under `Controllers` implement `IEndpoint` and register minimal API routes. `Controllers/Endpoint.cs` explicitly registers them; adding a class alone does not expose a route. Use `StudentController.cs` and `ResultHelper.cs` to trace handler execution and HTTP result conversion.

`Program.cs` and `ExtensionService` own service registration and middleware. GraphQL wiring is currently commented out; do not assume GraphQL files represent live endpoints or enable it incidentally. gRPC uses `Grpc/Protos/Identity.proto` and `IdentityGrpc.cs`; consider client compatibility when changing the contract.

EF migrations and the model snapshot live here, while the DbContext lives in Infrastructure. Keep one owner for migration generation. Build after API changes; runtime checks require database and authentication configuration from README.md. `/healthz` includes a database check, so an unavailable database can make it unhealthy.
