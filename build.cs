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

void RunTests() => RequireDotNet($"test --solution {solution} --configuration {configuration} --no-build --no-restore");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Bootstrap")
    .Does(() =>
{
    RequireDotNet("tool restore");
    RequireDotNet("restore AlumniService.slnx");
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
    .Does(() => RequireDotNet("run --project Source/Apps/AppHost"));

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
    .Does(() => RequireDotNet($"restore {solution}"));

Task("Build-Format-Generator")
    .IsDependentOn("Restore")
    .Does(() =>
    // dotnet format loads the default Debug workspace and needs the analyzer DLL.
    RequireDotNet("build Source/Libraries/Generators/Generators.csproj --configuration Debug --no-restore"));

Task("Test")
    .IsDependentOn("Build")
    .Does(RunTests);

Task("Check-Format")
    .IsDependentOn("Build-Format-Generator")
    .Does(() =>
{
    Information("Checking whitespace formatting against .editorconfig...");
    RequireDotNet($"format whitespace {solution} --no-restore --verify-no-changes");
    Information("Formatting check passed.");
});

Task("Lint")
    .IsDependentOn("Check-Format")
    .Does(() =>
{
    Information("Checking formatting, code style, and analyzer diagnostics at warning severity or higher...");
    RequireDotNet($"format {solution} --no-restore --verify-no-changes --severity warn");
    Information("Lint check passed.");
});

Task("CI-Build")
    .IsDependentOn("Lint")
    .Does(() =>
{
    var arguments = new ProcessArgumentBuilder()
        .Append("build")
        .AppendQuoted(solution)
        .Append("--configuration")
        .AppendQuoted(configuration)
        .Append("--no-restore --warnaserror -p:ContinuousIntegrationBuild=true");

    var exitCode = StartProcess("dotnet", new ProcessSettings
    {
        Arguments = arguments,
        WorkingDirectory = MakeAbsolute(Directory(".")),
    });
    if (exitCode != 0)
    {
        throw new InvalidOperationException($"CI build failed with exit code {exitCode}.");
    }
});

Task("CI-Test")
    .IsDependentOn("CI-Build")
    .Does(RunTests);

Task("CI")
    .IsDependentOn("CI-Test");

var migrationName = Argument("MigrationName", "Migration_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture));

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
        throw new InvalidOperationException($"Migration creation failed with exit code {exitCode}.");
    }
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
