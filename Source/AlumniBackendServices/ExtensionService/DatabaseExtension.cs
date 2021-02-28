using System;
using Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlumniBackendServices.ExtensionService
{
    public static class DatabaseExtension
    {
        public static void AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            var connection = configuration.GetConnectionString("AlumniConnection");

            if(env.IsDevelopment())
            {
                var password = configuration.GetConnectionString("Password");
                connection = string.Format(connection, password);
            }
            else
            {
                var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
                connection = string.Format(connection, password);
            }

             services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(connection, b => b.MigrationsAssembly("AlumniBackendServices")));
        }
    }
}
