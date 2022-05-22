using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TaskManager.API.Extensions;

namespace TaskManager.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Add framework services.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Task Manager REST API",
                    Version = "v1",
                    Description = "TaskManager REST API handles the requests to Create and Update the Tasks."
                });
            });

            // Register Application Depedencies
            services
                .AddDatabase(Configuration)
                .AddRepositories()
                .AddServices()
                .AddHandlers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Developer Exception Page Middleware
                app.UseDeveloperExceptionPage();

            }

            // Swagger Registrations
            app.UseSwagger(c => c.RouteTemplate = "/TaskManager/swagger/{documentName}/swagger.json");
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/TaskManager/swagger/v1/swagger.json", "Task Manager REST API V1");
                    c.RoutePrefix = "TaskManager/swagger";
                });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




        }
    }
}
