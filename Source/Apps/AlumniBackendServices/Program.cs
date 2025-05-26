using AlumniBackendServices.Controllers;
using AlumniBackendServices.ExtensionService;
using AlumniBackendServices.Grpc;
using AlumniBackendServices.Services;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
//     {
//         options.Listen(IPAddress.Parse("127.0.0.1"), 5000, listenOptions =>
//         {
//             listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
//         });
//     });

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<ISettingService>(new SettingService(builder.Configuration));
builder.Services.AddInfrastructureServices(new SettingService(builder.Configuration));
builder.Services.AddAuth();
builder.Services.AddApplicationOpenApi();
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddApplicationLogging(builder.Environment);

var app = builder.Build();

app.UseApplicationOpenApi();
app.UseApplication();
app.UseAuth();

// Add Controllers
StudentController.Add(app);
CompanyController.Add(app);
ExamController.Add(app);
FurtherStudiesController.Add(app);
FacultyController.Add(app);

// app.UseApplicationGraphQL();
app.MapGrpcService<IdentityGrpc>();

app.Run();
