using RobotService;
using Serilog;


Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
