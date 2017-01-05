using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ConsoleApplication.Services;

namespace ConsoleApplication
{
    public class IoC
    {
        public static IServiceProvider Services { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();

            // Adding my own service to the service collection
            services.AddSingleton<IMyService, MyService>();

            // Make strongly typed configuration available
            services.Configure<MyServiceConfiguration>(configuration.GetSection("MyService"));

            // Add required services for strongly typed configuration and logging
            services.AddOptions();
            services.AddLogging();

            Services = services.BuildServiceProvider();
        }
    }
}