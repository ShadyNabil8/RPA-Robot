using Serilog;
using Serilog.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using Serilog;

namespace rpa_robot.Classes
{
    internal class Initialization
    {
        public static Watcher WatcherObj = new Watcher();
        //public static WorkflowHandler HandlerObj = new WorkflowHandler();
        //public static Thread ListenerFromService = new Thread(AsynchronousSocketListener.StartListening);
        public static BackgroundWorker ListenerWorker = new BackgroundWorker();
        
        public static void AppInit()
        {

            CreateWorkflowFolders();
            LoggerInit();
            WatcherInit();
            SocketListenerInit();

        }
        public static void CreateWorkflowFolders()
        {
            try
            {
                if (!Directory.Exists(Globals.watcherPath))
                {
                    Directory.CreateDirectory(Globals.watcherPath);
                }
                if (!Directory.Exists(Globals.destinationPath))
                {
                    Directory.CreateDirectory(Globals.destinationPath);
                }
                if (!Directory.Exists(Globals.LogDirectoryPath))
                {
                    Directory.CreateDirectory(Globals.LogDirectoryPath);
                }
                if (!File.Exists(Globals.LogFilePath))
                {
                    using (File.Create(Globals.LogFilePath))
                    {

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        public static void WatcherInit()
        {
            WatcherObj.watcher.Path = Globals.watcherPath;
            WatcherObj.watcher.Created += WatcherObj.OnFileCreated;
            WatcherObj.watcher.EnableRaisingEvents = true;
            WatcherObj.FileCreated += WorkflowHandler.OnFileCreated;
        }
        public static void LoggerInit()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Globals.LogFilePath)
                .CreateLogger();
            }
            catch (Exception)
            { throw; }
        }
        public static void SocketListenerInit()
        {
            try
            {
                //ListenerFromService.Start();
                ListenerWorker.DoWork += Listener.Listen;
                ListenerWorker.RunWorkerAsync();
                Log.Information("Robot listener thread started");
            }
            catch (Exception)
            {

                Log.Information("Robot listener thread failed");
            }
        }
    }
}
