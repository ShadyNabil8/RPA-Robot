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
            checkLogFileSize();
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(Globals.LogPath)
               .CreateLogger();
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

        private static void checkLogFileSize()
        {
            FileInfo fileInfo = new FileInfo(Globals.LogPath);
            long fileSizeInBytes = fileInfo.Length;

            const long bytesInMegabyte = 1024 * 1024;
            long fileSizeInMegabytes = fileSizeInBytes / bytesInMegabyte;

            if (fileSizeInMegabytes > 100)
            {
                // Delete the file
                File.Delete(Globals.LogPath);
                Log.Information("Log File deleted successfully.");
            }
        }



    }
}
