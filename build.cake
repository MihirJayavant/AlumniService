#addin nuget:?package=Cake.EntityFrameworkCore&version=4.0.0

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var webapi = "./Source/Apps/AlumniBackendServices";
var core = "./Source/Libraries/Core";
var student = "./Source/Libraries/Alumni.Student";
var faculty = "./Source/Libraries/Alumni.Faculty";
var infrastructure = "./Source/Libraries/Infrastructure";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var clean = Task("Clean")
                .WithCriteria(c => HasArgument("rebuild"))
                .Does(() =>
                    {
                        CleanDirectory($"{webapi}/bin/{configuration}");
                        CleanDirectory($"{core}/bin/{configuration}");
                        CleanDirectory($"{student}/bin/{configuration}");
                        CleanDirectory($"{faculty}/bin/{configuration}");
                        CleanDirectory($"{infrastructure}/bin/{configuration}");
                    });

var build = Task("Build")
            .IsDependentOn(clean)
            .Does(() =>
                {
                    DotNetCoreBuild("./AlumniBackendServices.sln", new DotNetCoreBuildSettings
                    {
                        Configuration = configuration,
                    });
                });

var migrationName = Argument("MigrationName", "Migration_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));

Task("Add-Migration")
    .Does(() =>
{
    EfCoreMigrationsAdd(migrationName);
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
