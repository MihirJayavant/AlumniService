#:sdk Cake.Sdk

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var solution = "./AlumniService.slnx";

// All targets run from the repository root, alongside this file.
int RunDotNet(string arguments) => StartProcess("dotnet", new ProcessSettings
{
    Arguments = new ProcessArgumentBuilder().Append(arguments),
    WorkingDirectory = MakeAbsolute(Directory(".")),
});

void RequireDotNet(string arguments)
{
    var exitCode = RunDotNet(arguments);
    if (exitCode != 0)
    {
        throw new InvalidOperationException($"dotnet {arguments} failed with exit code {exitCode}.");
    }
}

void RunTests() => DotNetTest(solution, new DotNetTestSettings
{
    PathType = DotNetTestPathType.Solution,
    Configuration = configuration,
    NoBuild = true,
    NoRestore = true,
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Bootstrap")
    .Does(() =>
{
    DotNetToolRestore();
    DotNetRestore(solution);
});

Task("Doctor")
    .Does(() =>
{
    // global.json enforces SDK selection; dotnet reports an error if unavailable.
    RequireDotNet("--version");
    RequireDotNet("ef --version");

    if (RunDotNet("dev-certs https --check") != 0)
    {
        Warning("Local HTTPS needs a development certificate: dotnet dev-certs https --trust");
    }

    try
    {
        var dockerExitCode = StartProcess("docker", new ProcessSettings
        {
            Arguments = "info",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        });
        if (dockerExitCode != 0)
        {
            Warning("Aspire requires a running Docker-compatible container runtime.");
        }
    }
    catch (Exception)
    {
        Warning("Docker is unavailable. Aspire requires a running Docker-compatible container runtime.");
    }

    Information("Configure AppHost Parameters:pg-user and Parameters:pg-password in user secrets.");
    Information("Configure API Authentication:Secret, Authentication:ValidAudience, and Authentication:ValidIssuer locally.");
    Information("Secret values are not inspected. See README.md for setup.");
});

Task("Run-Local")
    .Does(() => DotNetRun("Source/Apps/AppHost"));

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

Task("Restore")
    .Does(() => DotNetRestore(solution));

Task("Build-Format-Generator")
    .IsDependentOn("Restore")
    .Does(() =>
    // dotnet format loads the default Debug workspace and needs the analyzer DLL.
    DotNetBuild("Source/Libraries/Generators/Generators.csproj", new DotNetBuildSettings
    {
        Configuration = "Debug",
        NoRestore = true,
    }));

Task("Test")
    .IsDependentOn("Build")
    .Does(RunTests);

Task("Format")
    .IsDependentOn("Build-Format-Generator")
    .Does(() =>
{
    Information("Checking formatting, code style, and analyzer diagnostics at warning severity or higher...");
    DotNetFormat(solution, new DotNetFormatSettings
    {
        NoRestore = true,
        VerifyNoChanges = true,
        Severity = DotNetFormatSeverity.Warning,
    });
    Information("Format check passed.");
});

Task("CI-Build")
    .IsDependentOn("Format")
    .Does(() => DotNetBuild(solution, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoRestore = true,
        ArgumentCustomization = arguments => arguments
            .Append("--warnaserror -p:ContinuousIntegrationBuild=true"),
    }));

Task("CI-Test")
    .IsDependentOn("CI-Build")
    .Does(RunTests);

Task("CI")
    .IsDependentOn("CI-Test");

var migrationName = Argument("MigrationName", "Migration_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture));

Task("Add-Migration")
    .Does(() => RequireDotNet(new ProcessArgumentBuilder()
        .Append("ef migrations add")
        .AppendQuoted(migrationName)
        .Append("--project Source/Apps/AlumniBackendServices")
        .Append("--startup-project Source/Apps/AlumniBackendServices")
        .Append("-- --environment Development")
        .Render()));

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
