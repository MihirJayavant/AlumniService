var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("pg-user", secret: true);
var password = builder.AddParameter("pg-password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
    .WithPgAdmin()
    .WithDataVolume()
    .AddDatabase("alumni-db");

builder.AddProject<Projects.AlumniBackendServices>("alumni-service")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
