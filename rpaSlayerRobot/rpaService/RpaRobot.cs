using System.ServiceProcess;
using RobotService;
using Serilog;
using System.Timers;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace rpaService
{
    public partial class RpaRobot : ServiceBase
    {
        private RobotConnection Robot;
        public RpaRobot()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();

            Log.Information("Service is started");

            Robot = new RobotConnection();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
           // while (true)
           // {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                Log.Information("Service is Sending to Robot");
                Robot.RobotStartClient();
                
           // }
        }

        protected override void OnStop()
        {
            Log.Information("Service is stoped");
        }

        protected override void OnContinue()
        {
            
        }
    }
}
