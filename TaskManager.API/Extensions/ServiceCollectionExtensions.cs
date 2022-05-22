using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.API.Commands;
using TaskManager.API.Constants;
using TaskManager.API.Services;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.API.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Repositories
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services.AddScoped<ITaskRepository, TaskRepository>();
        }

        /// <summary>
        /// Register Database
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services
            , IConfiguration configuration)
        {
            return services.AddDbContext<TaskManagerDbContext>(options =>
                     options.UseSqlServer(configuration.GetConnectionString(ApiConstants.DBConnectionString)));
        }

        /// <summary>
        /// Register Services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services.AddScoped<ITaskManagerService, TaskManagerService>();
        }

        /// <summary>
        /// Register Handlers
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            return services.AddMediatR(typeof(AddTaskCommandHandler))
                .AddMediatR(typeof(UpdateTaskCommandHandler));
        }
    }
}
