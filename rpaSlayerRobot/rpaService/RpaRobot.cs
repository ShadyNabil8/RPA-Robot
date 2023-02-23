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

namespace rpaService
{
    public partial class rpaService : ServiceBase
    {
        private AsynchronousSocketListener RobotAsClient;
        //private AsynchronousClient RobotAsServer;
        

        public rpaService()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();

            Log.Information("Service is started");

            RobotAsClient = new AsynchronousSocketListener();
            //RobotAsServer = new AsynchronousClient();
            
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            

            new Thread(() =>
            {
                RobotAsClient.StartListening();

            }).Start();


            new Thread(() =>
            {
                AsynchronousClient RobotAsServer = new AsynchronousClient();
                FtpClient ftp = new FtpClient();
                ftp.DownloadFiles();
                
                while (true)
                {
                    RobotAsServer.StartClient();
                    Thread.Sleep(3000);
                }


                }).Start();

            






                /*Thread OrchestratorThread = new Thread(() =>
                {
                    try
                    {
                        Orchestrator.ReciveByTCP(IPAddress.Parse(""), 9001);
                        Log.Information("SERVICE IS READY TO LISTEN FROM ORCHESTRATOR!");
                        while (true)
                        {
                            Log.Information("SERVICE IS LISTENING FROM ORCHESTRATOR!");
                            Socket handler = Robot.ServiceListener.Accept();
                            Log.Information("CONNEDTED TO ORCHESTRATOR!");
                            try
                            {
                                byte[] buffer = new byte[255];
                                int rec = handler.Receive(buffer);
                                if (rec > 0)
                                {
                                    Array.Resize(ref buffer, rec);
                                    Log.Information("SERVICE RECEIVED : " + Encoding.ASCII.GetString(buffer, 0, rec));
                                }
                                else
                                {
                                    throw new SocketException();
                                }
                            }
                            catch
                            {
                                Log.Error("CONNECTION WITH ORCHESTRATOR FAILED");
                            }

                            handler.Shutdown(SocketShutdown.Both);
                            handler.Close();
                        }
                    }
                    catch
                    {
                        Log.Error("SERVICE CANNOT CONNECT TO ORCHESTRATOR");
                    }


                });
                OrchestratorThread.Start();*/

            }

        protected override void OnStop()
        {
            Log.Information("SERVICE IS STOPPED");
        }

        protected override void OnContinue()
        {

        }

        protected override void OnShutdown()
        {
            Log.Information("SERVICE SHUT");
        }
        
    }
}
