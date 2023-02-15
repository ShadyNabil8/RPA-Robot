using Serilog;
using System.Runtime.CompilerServices;

namespace RobotService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private RobotConnection Robot;
       

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Service is started");

            Robot = new RobotConnection();

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Service is stoped");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Log.Information("Service is Sending to Robot");
                Robot.RobotStartClient();
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}