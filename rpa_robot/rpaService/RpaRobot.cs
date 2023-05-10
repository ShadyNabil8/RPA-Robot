using System.ServiceProcess;
using Serilog;
using rpaService.Classes;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService
{
    public partial class rpaService : ServiceBase
    {
        
        public rpaService()
        {
            InitializeComponent();

            // Initialize the logging process
            LogInit();
  
        }

        protected override void OnStart(string[] args)
        {
            Log.Information(Info.SERVICE_STARTED);
            Globals.ListenerFromRobot.Start();
            Globals.LoggingProcess.Start();
            Orchestrator.MakeAuthenticationAsync();
        }
        protected override void OnStop()
        {
            Log.Information(Info.SERVICE_STOPED);
        }

        protected override void OnContinue()
        {
            Log.Information(Info.SERVICE_CONT);
        }

        protected override void OnShutdown()
        {
            Log.Information(Info.SERVICE_SHUTDOWN);
        }
        private void LogInit()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(Globals.LogPath)
               .CreateLogger();
        }

        
    }
}
