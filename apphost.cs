#:sdk Aspire.AppHost.Sdk@13.6.0
#:package Aspire.Hosting.PostgreSQL
#:package Aspire.Hosting.Dotnet
#:property AspireUseCliBundle=true

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("pg-user", secret: true);
var password = builder.AddParameter("pg-password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password, port: 5432)
    .WithEnvironment("POSTGRES_DB", "alumni-db")
    .WithPgAdmin()
    .WithDataVolume()
    .AddDatabase("alumni-db");

builder.AddDotnetProject("alumni-service", "Source/Apps/AlumniBackendServices/AlumniBackendServices.csproj")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
