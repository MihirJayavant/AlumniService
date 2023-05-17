using AlumniBackendServices.Controllers;
using AlumniBackendServices.ExtensionService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddApplicationSwagger(builder.Configuration);
builder.Services.AddMediatorServices();
builder.Services.AddDatabase(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddApplicationLogging(builder.Environment);
builder.Services.AddApplicationGraphQL();

var app = builder.Build();

// app.UseSwagger();
// app.UseSwaggerUI();

app.UseApplication();
// app.UseHttpsRedirection();
app.UseAuthorization();

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
