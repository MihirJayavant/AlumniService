using Microsoft.EntityFrameworkCore;

namespace AlumniBackendServices.ExtensionService;

public static class LoggingExtension
{
    public static void AddApplicationLogging(this IServiceCollection services, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddLogging
            (
                service => LoggerFactory.Create(builder => builder.AddConsole())
            );
            return;
        }

        services.AddLogging
        (
            service => LoggerFactory
                .Create(builder => builder
                    .AddFilter
                    (
                        (category, level) =>
                            category != DbLoggerCategory.Database.Command.Name
                    )
                    .AddConsole())
        );
    }
}
