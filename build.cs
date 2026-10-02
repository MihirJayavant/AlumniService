#:sdk Cake.Sdk

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var solution = "./AlumniService.slnx";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var clean = Task("Clean")
                .WithCriteria(c => HasArgument("rebuild"))
                .Does(() => DotNetClean(solution, new DotNetCleanSettings
                {
                    Configuration = configuration,
                }));

var build = Task("Build")
            .IsDependentOn(clean)
            .Does(() => DotNetBuild(solution, new DotNetBuildSettings
            {
                Configuration = configuration,
            }));

var migrationName = Argument("MigrationName", "Migration_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));

Task("Add-Migration")
    .Does(() =>
{
    var arguments = new ProcessArgumentBuilder()
        .Append("ef migrations add")
        .AppendQuoted(migrationName)
        .Append("--project Source/Apps/AlumniBackendServices")
        .Append("--startup-project Source/Apps/AlumniBackendServices")
        .Append("-- --environment Development");

    var exitCode = StartProcess("dotnet", new ProcessSettings
    {
        Arguments = arguments,
        WorkingDirectory = MakeAbsolute(Directory(".")),
    });

    if (exitCode != 0)
    {
        throw new Exception($"Migration creation failed with exit code {exitCode}.");
    }
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
