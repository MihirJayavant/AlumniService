using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlumniBackendServices.ExtensionService
{
    public static class LoggingExtension
    {
        public static void AddApplicationLogging(this IServiceCollection services, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                services.AddLogging
                (
                    service => LoggerFactory.Create(builder => builder.AddConsole())
                );
            }
            else
            {
                services.AddLogging
                (
                    service => LoggerFactory
                                    .Create(builder => builder
                                    .AddFilter
                                    (
                                        (category, level) =>
                                                category != DbLoggerCategory.Database.Command.Name
                                    )
                                    .AddConsole() )
                );
            }
        }
    }
}
