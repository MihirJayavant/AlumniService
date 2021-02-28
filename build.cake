var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

var webapi = "./Source/AlumniBackendServices";
var core = "./Source/Core";
var database = "./Source/Database";
var infrastructure = "./Source/Infrastructure";
var proxy = "./Source/ProxyApp";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

var clean = Task("Clean")
                .WithCriteria(c => HasArgument("rebuild"))
                .Does(() =>
                    {
                        CleanDirectory($"{webapi}/bin/{configuration}");
                        CleanDirectory($"{core}/bin/{configuration}");
                        CleanDirectory($"{database}/bin/{configuration}");
                        CleanDirectory($"{infrastructure}/bin/{configuration}");
                        CleanDirectory($"{proxy}/bin/{configuration}");
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


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
