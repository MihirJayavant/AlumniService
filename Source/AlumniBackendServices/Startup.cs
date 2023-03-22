using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AlumniBackendServices.ExtensionService;
using Microsoft.Extensions.Hosting;

namespace AlumniBackendServices
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAuth(Configuration);
            services.AddApplicationSwagger(Configuration);
            services.AddDatabase(Configuration, Env);
            services.AddApplicationServices(Configuration);
            services.AddApplicationLogging(Env);
            services.AddApplicationGraphQL();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuth();
            app.UseApplicationSwagger(Configuration);
            app.UseApplication();
            app.UseApplicationGraphQL();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
