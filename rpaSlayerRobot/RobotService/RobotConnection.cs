using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobotService
{
    internal class RobotConnection
    {
        const int RobotListiningPort = 9000;
        public RobotConnection()
        {

        }

        public void RobotStartClient()
        {
            // Connect to a Robot
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, RobotListiningPort);

            // Create a TCP/IP  socket.
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            byte[] bytes = new byte[1024];

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                sender.Connect(remoteEP);

                // log connected

                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes("Run Xaml");


                // Send the data through the socket.
                int bytesSent = sender.Send(msg);

                //Receive the response from the remote device.
                //int bytesRec = sender.Receive(bytes);
                //Console.WriteLine("Echoed test = {0}",
                //Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                //log
            }
            catch (SocketException se)
            {
                //log
            }
            catch (Exception e)
            {
                //log
            }

        }

    }

}

