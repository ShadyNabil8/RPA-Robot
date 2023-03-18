using System.ServiceProcess;
using Serilog;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.Net;
using FluentFTP;

namespace rpaService
{
    public partial class rpaService : ServiceBase
    {
        private AsynchronousSocketListener ServiceAsyncListenerFromRobot = new AsynchronousSocketListener();
        private AsynchronousClient ServiceAsyncClientFromRobot = new AsynchronousClient();
        public static Thread ServiceAsyncListenerFromRobotThread;
        public static Thread ServiceAsyncListenerFromRobotThreadHnadler;
        public rpaService()
        {
            InitializeComponent();
            // Initialize the logging process
            LogInit();
        }

        protected override void OnStart(string[] args)
        {
            Log.Information("SERVICE-STARTED");
            ServiceAsyncListenerFromRobotThread = new Thread(ServiceAsyncListenerFromRobotFun);
            ServiceAsyncListenerFromRobotThread.Start();
            ServiceAsyncListenerFromRobotThreadHnadler = new Thread(ServiceAsyncListenerFromRobotThreadHnadlerFun);
            ServiceAsyncListenerFromRobotThreadHnadler.Start(); 
        }
        protected override void OnStop()
        {
            Log.Information("SERVICE-STOPPED");
        }

        protected override void OnContinue()
        {
            Log.Information("SERVICE-CONTINUED");
        }

        protected override void OnShutdown()
        {
            Log.Information("SERVICE-SHUTDOWN");
        }

        private void LogInit()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();
        }

        private void ServiceAsyncListenerFromRobotFun() 
        {
            ServiceAsyncListenerFromRobot.StartListening();
        }

        private void ServiceAsyncListenerFromRobotThreadHnadlerFun() 
        {
            while (true)
            {
                if (ServiceAsyncListenerFromRobot.ProcessQueue.Count > 0)
                {
                    Log.Information(ServiceAsyncListenerFromRobot.ProcessQueue.Dequeue() + "form Q");
                    //ServiceAsyncClientFromRobot.StartClient();
                }
                else
                {
                    //== THIS LINE IS WRITTEN TO AVOID THE OVEDHEAD DUE TO THE WHILE LOOP, LOOPING ON NOTHING ==//
                    Thread.Sleep(5000);
                }
            }
        }
    }
}
