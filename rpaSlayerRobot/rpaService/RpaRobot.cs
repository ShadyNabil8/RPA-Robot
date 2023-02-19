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
        private TcpSocket Robot;
        private TcpSocket Orchestrator;

        public rpaService()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log")
               .CreateLogger();

            Log.Information("Service is started");

            Robot = new TcpSocket();
            Orchestrator = new TcpSocket();
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Thread RobotThread = new Thread(() =>
            {
                try
                {
                    Robot.ReciveByTCP(Robot.ServiceListeningIP, 9000);
                    Log.Information("SERVICE IS READY TO LISTEN FROM ROBOT!");
                    while (true)
                    {
                        Log.Information("SERVICE IS LISTENING FROM ROBOT!");
                        Socket handler = Robot.ServiceListener.Accept();
                        Log.Information("CONNEDTED TO ROBOT!");
                        try
                        {
                            byte[] buffer = new byte[255];
                            int rec = handler.Receive(buffer);
                            if (rec > 0)
                            {
                                Array.Resize(ref buffer, rec);
                                Log.Information("SERVICE RECEIVED : " + Encoding.ASCII.GetString(buffer, 0, rec));
                                Robot.SendByTCP(Robot.RobotListeningIP, 9001, "ACK");
                            }
                            else
                            {
                                throw new SocketException();
                            }
                        }
                        catch
                        {
                            Log.Error("CONNECTION WITH ROBOT FAILED");
                        }

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                        
                    }
                }
                catch
                {
                    Log.Error("SERVICE CANNOT CONNECT TO ROBOT");
                }

            });
            RobotThread.Start();
            
            Thread OrchestratorThread = new Thread(() =>
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
            OrchestratorThread.Start();

        }

        protected override void OnStop()
        {
            Log.Information("SERVICE IS STOPPED");
        }

        protected override void OnContinue()
        {

        }
    }
}
