using AlumniBackendServices.Controllers;
using AlumniBackendServices.ExtensionService;
using AlumniBackendServices.Grpc;
using AlumniBackendServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<ISettingService>(new SettingService(builder.Configuration));
builder.Services.AddInfrastructureServices(new SettingService(builder.Configuration));
builder.Services.AddApplicationOpenApi(builder);
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddApplicationLogging(builder.Environment);

var app = builder.Build();

app.UseApplicationOpenApi();
app.UseApplication();
app.UseAuth();

// Add Controllers
app.AddControllers();

// app.UseApplicationGraphQL();
app.MapGrpcService<IdentityGrpc>();

app.Run();
