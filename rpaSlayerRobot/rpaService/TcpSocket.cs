using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace rpaService
{

    public class TcpSocket
    {
        static IPHostEntry host = Dns.GetHostEntry("localhost");
        public IPAddress ServiceListeningIP  = host.AddressList[0];
        public IPAddress RobotListeningIP    = host.AddressList[1];
        public Socket ServiceListener;
        public Socket ServiceClient;

        public void ReciveByTCP(IPAddress IP,int Port) 
        {
            ServiceListener = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEB = new IPEndPoint(IP, Port);
            ServiceListener.Bind(remoteEB);
            ServiceListener.Listen(1);
        }

        public void SendByTCP(IPAddress IP, int Port, String data)
        {
            ServiceClient = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEB = new IPEndPoint(IP, Port);
            byte[] msg = Encoding.ASCII.GetBytes(data);
            try
            {
                ServiceClient.Connect(remoteEB);
                Log.Information("SERVICE CONNECTED TO SERVICE AS CLIENT");
                int bytesSent = ServiceClient.Send(msg);
                ServiceClient.Shutdown(SocketShutdown.Both);
                ServiceClient.Close();
            }
            catch
            {
                throw new SocketException();

            }
        }

    }

}

