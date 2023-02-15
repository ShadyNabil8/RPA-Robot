using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Serilog;

namespace rpaSlayerRobot
{
    internal class TcpSocket
    {
        
        const int RobotListiningPort = 9000;
        
        public void StartServer()
        {
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, RobotListiningPort);

            try
            {

                // Create a Socket that will use Tcp protocol
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 10 requests at a time
                listener.Listen(10);

                //Console.WriteLine("Waiting for a connection...");
                while (true)
                {
                    // waiting for a call
                    Log.Information("Robot listing in port {0}", RobotListiningPort);
                    Socket handler = listener.Accept();
                    Log.Information("Robot is connected with the service");


                    // Use a different thread for each client


                    // Incoming data from the client.
                    string data = null;
                    byte[] bytes = null;
                    bool TerminationFlag = false;


                    while (true)
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.Equals("Run Xaml"))
                        {
                            Handler.RunWorkFlow();
                            Log.Information("WF is executed");
                            break;
                        }
                        else
                        {
                            // log
                            TerminationFlag = true;
                            break;
                        }
                    }


                    //Console.WriteLine("Text received : {0}", data);
                    //byte[] msg = Encoding.ASCII.GetBytes(data);
                    //handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Log.Information("Connection off");


                    if (TerminationFlag)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Information("Cannot initiate the TCP connection");
            }

        }

    }
}
