# ASP.NET Core console app with Dependency Injection and Serilog

This sample shows how to create an [ASP.NET Core](https://www.microsoft.com/net/core) console application with [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection) support and logging with [Serilog](https://serilog.net/).


In an ASP.NET Core *Web Application* the DI stuff is automatically wired up for you by the `WebHostBuilder`, so the only thing you need to do is to configure the services. However, in a console application you need to handle some more of the pieces yourself. 

### Create a new console app

I assume you already have the .NET Core SDK installed. If not, go to [dot.net](https://dot.net) and install it.

The first step is to create a new console app. Create a new directory and run `dotnet new`

```
C:\projects\MyConsoleApp> dotnet new
Created new C# project in C:\projects\MyConsoleApp.
````


### Required dependencies
You need to add a few dependencies in your `project.json`. Here is the list of dependencies needed:

```
"dependencies": {
  "Microsoft.Extensions.DependencyInjection": "1.1.0",
  "Microsoft.Extensions.Configuration": "1.1.0",
  "Microsoft.Extensions.Configuration.Json": "1.1.0",
  "Microsoft.Extensions.Options.ConfigurationExtensions": "1.1.0",
  "Microsoft.Extensions.Logging": "1.1.0",
  "Serilog": "2.3.0",
  "Serilog.Extensions.Logging": "1.3.1",
  "Serilog.Sinks.Literate": "2.0.0"
},
```


### Create a static instance of `IServiceProvider`

I'm not sure if this is the optimal way to do this, but it works for me :) Create a static class where you add a public static property for your `IServiceProvider`. This class has a static method `ConfigureServices` where you need to add all the services you want to be available to your application.

The method takes in a `IConfiguration` to enable configuration via the built in configuration system.


### Accessing services

From the `Main` method you can get services from the service provider after it has been configured.

You use the static property `Services` to request services like this:

```
var service = IoC.Services.GetService<IMyService>();
service.WriteToLog();
```

The nice thing is that the services provider will automatically take care of injecting services you depend on further down the stack. In `IMyService` we want to read some configuration and do some logging. These dependencies are automatically injected as constructor parameters like this:

```
public MyService(IOptions<MyServiceConfiguration> configuration, ILogger<MyService> logger)
{
  _configuration = configuration.Value;
  _logger = logger;
}
```

### Add logging using Serilog

To make sure the `ILogger<MyService>` is properly injected to your service, you need to set up a couple of things.

The built-in interfaces are made available by adding the NuGet package  `Microsoft.Extensions.Logging`

We create a `ConfigureLogger` method that is called _after_ you have run `ConfigureServices`. This method specifies that you want to use Serilog and that you want to use the LiteralConsole sink for outputting log statements.

It's important that you run `ConfigureServices` before you configure the logger because you need the services made available by `services.AddLogging();` in order to configure Serilog.

```
private static void ConfigureLogger()
{
  Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.LiterateConsole()
    .CreateLogger();

  var loggerFactory = IoC.Services.GetRequiredService<ILoggerFactory>();
  loggerFactory.AddSerilog();
}    
```

### Strongly typed configuration

To get strongly typed configuration injected to your service as a constructor parameter like this:

`public MyService(IOptions<MyServiceConfiguration> configuration)`

you need to add the NuGet packages `Microsoft.Extensions.Configuration`,    `Microsoft.Extensions.Configuration.Json` and `Microsoft.Extensions.Options.ConfigurationExtensions`.

I've added a method for reading configuration from `appsettings.json` like this:

```
public static IConfiguration Configure()
{
  var builder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json");

  return builder.Build();
}
```

You could also read configuration from environment variables, xml files or any other source you might find useful.

To make the configuration available through DI, you need to add it to the service container like this:

```
services.Configure<MyServiceConfiguration>(configuration.GetSection("MyService"));
```

and you need to call `services.AddOptions();` to add some built-in services for handling options.

You can read more about configuration in [the official docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration).


### Result

If you run the application, you will see that the service is resolved from the service collection and when you call the `WriteToLog()` method on the service, it will log a message to the console with a value from the configuration file.


```
C:\projects\MyConsoleApp [master ≡ ]> dotnet run
[12:25:04 INF] Value from config file: this is my value
```


