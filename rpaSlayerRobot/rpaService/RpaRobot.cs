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
        private AsynchronousSocketListener RobotAsClient;
        private AsynchronousClient RobotAsServer;
        public rpaService()
        {
            // Initialize the logging process
            LogInit();

            RobotAsClient = new AsynchronousSocketListener();
            RobotAsServer = new AsynchronousClient();

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Information("Service is started");

            new Thread(() =>
            {
                RobotAsClient.StartListening();

            }).Start();


            new Thread(() =>
            {

                while (true)
                {
                    if(RobotAsClient.ProcessQueue.Count > 0) 
                    {
                        Log.Information(RobotAsClient.ProcessQueue.Dequeue() + "form Q");
                    }
                }
            }).Start();

            new Thread(() =>
            {
                while (true)
                {
                    RobotAsServer.StartClient();
                    Thread.Sleep(3000);
                }
            }).Start();
        }
        protected override void OnStop()
        {
            Log.Information("SERVICE IS STOPPED");
        }

        protected override void OnContinue()
        {
            Log.Information("SERVICE IS Continued");
        }

        protected override void OnShutdown()
        {
            Log.Information("SERVICE SHUT");
        }

        private void LogInit()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();
        }

    }
}
