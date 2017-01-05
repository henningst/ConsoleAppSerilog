using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication.Services 
{
    public class MyService : IMyService
    {
        private MyServiceConfiguration _configuration;
        private ILogger<MyService> _logger;

        public MyService(IOptions<MyServiceConfiguration> configuration, ILogger<MyService> logger)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }
        public void WriteToLog()
        {
            _logger.LogInformation($"Value from config file: {_configuration.ConfigValue}");
        }
    }
}