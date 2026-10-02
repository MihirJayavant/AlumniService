using System.Diagnostics;

namespace AppHost;

internal static class PostgresResourceExtension
{
    extension(IResourceBuilder<PostgresDatabaseResource> builder)
    {
        public IResourceBuilder<PostgresDatabaseResource> WithMigrationCommand(
)
        {
            builder.WithCommand("postgres-migration", "Migration", context => OnRunMigrationCommandAsync(builder, context),
                new CommandOptions()
                {
                    IconName = "AnimalRabbitOff",
                    IconVariant = IconVariant.Filled,
                });

            return builder;
        }
    }

    private static async Task<ExecuteCommandResult> OnRunMigrationCommandAsync(
        IResourceBuilder<PostgresDatabaseResource> builder,
        ExecuteCommandContext context)
    {
        RunCommand("dotnet", "ef database update");
        return CommandResults.Success();
    }

    static void RunCommand(string fileName, string arguments)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        Console.WriteLine("Output:\n" + output);
        Console.WriteLine("Error:\n" + error);
    }
}
