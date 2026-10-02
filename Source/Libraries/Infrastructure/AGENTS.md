# Persistence and Identity

`ApplicationContext` implements the domain context interfaces and explicitly applies feature configurations. New persisted entities may require a DbSet, domain interface changes, and configuration registration here.

`ConfigureServices.cs` registers PostgreSQL, domain context adapters, Identity, JWT validation, authorization policies, and database health checks. Check the API's `Services/SettingService.cs` for connection-string precedence rather than inventing another configuration path.

Migrations belong to `Source/Apps/AlumniBackendServices`, as configured by `MigrationsAssembly`. Use the alumni-migration skill when changing the database model. One agent owns the context, migration, and snapshot integration. Generating a migration does not authorize applying it to a database.
