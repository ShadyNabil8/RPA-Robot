using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Initialization
    {
        public static Thread ListenerFromRobot = new Thread(Listener.Listen);
        public static void ServiceInit()
        {
            CreateWorkflowFolders();
            LoggerInit();
            TimerInit();
            SocketListenerInit();
        }
        public static void LoggerInit()
        {
            Log.Logger = new LoggerConfiguration()
           .WriteTo.File(Globals.LogFilePath)
           .CreateLogger();
        }
        public static void TimerInit()
        {
            try
            {
                Reconnector.timer = new Timer(Reconnector.TimerElapsed, null, Reconnector.timeoutDuration, Timeout.Infinite);
                Log.Information("Timer started");
            }
            catch (Exception)
            {

                Log.Information("Timer init failed");
            }
        }
        public static void SocketListenerInit()
        {
            try
            {
                ListenerFromRobot.Start();
                Log.Information("Robot listener thread started");
            }
            catch (Exception)
            {

                Log.Information("Robot listener thread failed");
            }
        }
        public static void CreateWorkflowFolders()
        {
            if (!Directory.Exists(Globals.LogDirectoryPath))
            {
                Directory.CreateDirectory(Globals.LogDirectoryPath);
            }
            if (!Directory.Exists(Globals.watcherPath))
            {
                Directory.CreateDirectory(Globals.watcherPath);
            }
            if (!File.Exists(Globals.LogFilePath))
            {
                using (File.Create(Globals.LogFilePath))
                {
                }
            }
        }
    }
}
