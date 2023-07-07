using System.ServiceProcess;
using Serilog;
using rpaService.Classes;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using WebSocketSharp;

namespace rpaService
{
    public partial class rpaService : ServiceBase
    {

        public rpaService()
        {
            InitializeComponent();
            
            try
            {
                Handler.CreateWorkflowFolders();
            }
            catch (Exception ex)
            {
                //Log.Information("Error creating folder: " + ex.Message);
            }
            try
            {
                Log.Logger = new LoggerConfiguration()
               .WriteTo.File(Globals.LogFilePath)
               .CreateLogger();
            }
            catch (Exception)
            { throw; }

            try
            {
                Handler.checkLogFileSize();
            }
            catch (Exception)
            {
                Log.Information("Error while checking the log file size!");
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Information(Info.SERVICE_STARTED);
            Globals.ListenerFromRobot.Start();
            Globals.LoggingProcess.Start();
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
        
    }
}
