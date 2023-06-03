using AlumniBackendServices.Controllers;
using AlumniBackendServices.ExtensionService;
using AlumniBackendServices.Services;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<ISettingService>(new SettingService(builder.Configuration));
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(new SettingService(builder.Configuration));
builder.Services.AddAuth();
builder.Services.AddApplicationSwagger(builder.Configuration);
builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddApplicationLogging(builder.Environment);

var app = builder.Build();

app.UseApplication();
// app.UseHttpsRedirection();

app.UseApplicationSwagger(builder.Configuration);
app.UseAuth();

IdentityController.Add(app);
StudentController.Add(app);
CompanyController.Add(app);
ExamController.Add(app);
FurtherStudiesController.Add(app);
FacultyController.Add(app);

// app.UseApplicationGraphQL();

app.MapControllers();


app.Run();
