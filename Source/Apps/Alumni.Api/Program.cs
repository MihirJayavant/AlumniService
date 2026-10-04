using Alumni.Api.Controllers;
using Alumni.Api.ExtensionService;
using Alumni.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<ISettingService>(new SettingService(builder.Configuration));
builder.Services.AddInfrastructureServices(new SettingService(builder.Configuration));
builder.Services.AddApplicationOpenApi(builder);
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddApplicationGrpc();
builder.Services.AddApplicationGraphQL();
builder.Services.AddApplicationLogging(builder.Environment);

var app = builder.Build();

app.UseApplicationOpenApi();
app.UseApplication();
app.UseAuth();

// Add Controllers
app.AddControllers();

app.MapApplicationGraphQL();
app.MapApplicationGrpc();

app.Run();
