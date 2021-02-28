using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AlumniBackendServices
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        // private static void CreateDbIfNotExists(IHost host)
        // {
        //     using var scope = host.Services.CreateScope();
        //     var services = scope.ServiceProvider;

        //     try
        //     {
        //         var context = services.GetRequiredService<ApplicationContext>();
        //         context.Database.EnsureCreated();
        //     }
        //     catch (Exception ex)
        //     {
        //         var logger = services.GetRequiredService<ILogger<Program>>();
        //         logger.LogError(ex, "An error occurred creating the DB.");
        //     }
        // }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

    }
}
