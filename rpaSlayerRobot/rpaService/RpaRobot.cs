using System.ServiceProcess;
using Serilog;
using rpaService.Classes;

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
            Globals.ServiceAsyncListenerFromRobotThread.Start();
            Globals.ServiceAsyncListenerFromRobotThreadHnadler.Start(); 
        }
        protected override void OnStop()
        {
            Log.Information(Info.SERVICE_STARTED);
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
