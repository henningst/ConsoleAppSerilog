using System.IO;
using ConsoleApplication.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = Configure();
            IoC.ConfigureServices(configuration);
            ConfigureLogger();

            var service = IoC.Services.GetService<IMyService>();
            service.WriteToLog();
        }

        public static IConfiguration Configure()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            
            return builder.Build();
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            var loggerFactory = IoC.Services.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddSerilog();
        }        
    }
}
