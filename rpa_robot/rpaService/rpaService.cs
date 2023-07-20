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
        Thread ListenerFromRobot;
        Thread LoggingProcess;
        public rpaService()
        {
            InitializeComponent();

            try
            {
                Handler.CreateWorkflowFolders();
            }
            catch (Exception)
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
            {
                Log.Error("Error in creating logger");
            }

            try
            {
                Handler.checkLogFileSize();
            }
            catch (Exception)
            {
                Log.Information("Error while checking the log file size!");
            }
            try
            {
                Reconnect.timer = new Timer(Reconnect.TimerElapsed, null, Reconnect.timeoutDuration, Timeout.Infinite);
                Log.Information("Timer started");
            }
            catch (Exception)
            {

                Log.Information("Timer init failed");
            }
            try
            {
                ListenerFromRobot = new Thread(Handler.ListenerFromRobothHandler);
                Log.Information("Robot listener thread started");
            }
            catch (Exception)
            {

                Log.Information("Robot listener thread failed");
            }
            try
            {
                LoggingProcess = new Thread(Handler.LoggingProcessHandler);
                Log.Information("loggingr thread started");
            }
            catch (Exception)
            {

                Log.Information("loggingr thread failed");
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Information(Info.SERVICE_STARTED);
            ListenerFromRobot.Start();
            LoggingProcess.Start();
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
