using System.Net;
using System.Net.Sockets;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

// Socket Listener acts as a server and listens to the incoming
// messages on the specified port and protocol.
public class SocketListener
{
    public static int Main(String[] args)
    {
        LogsInit();
        StartServer();
        return 0;
    }

    public static void CallWorkFlow()
    {
        System.Diagnostics.Process.Start(@"E:\CSE\4th comp\grad.Proj\WorkFlowRunner\WorkFlowRunner\bin\Debug\WorkFlowRunner.exe");
    }

    public static void LogsInit()
    {
        //initialize the layout of the logs
        var Layout = new PatternLayout();
        Layout.ConversionPattern = "%-5level %date{ISO8601} \"%message\"%newline";
        Layout.ActivateOptions();

        // initialize the file appender
        var Appender = new FileAppender();
        Appender.Name = "RobotApppender";
        Appender.Layout = Layout;
        Appender.Threshold = Level.All;
        Appender.AppendToFile = true;
        Appender.File = "E:\\CSE\\4th comp\\grad.Proj\\logs\\RobotLog.log";
        Appender.ActivateOptions();

        // initialize the configuration
        BasicConfigurator.Configure(Appender);

    }

    public static void StartServer()
    {
        // Get Host IP Address that is used to establish a connection
        // In this case, we get one IP address of localhost that is IP : 127.0.0.1
        // If a host has multiple addresses, you will get a list of addresses
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAddress = host.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        // create an instance of the logger
        ILog logger = LogManager.GetLogger(typeof(SocketListener));

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
                Socket handler = listener.Accept();

                logger.Info("incomming call.. begin the session");

                // Incoming data from the client.
                string data = null;
                byte[] bytes = null;
                bool termination = false;

                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.Equals("Run Xaml"))
                    {
                        CallWorkFlow();
                        logger.Info("WorkFlow is executed");
                        break;
                    }
                    else
                    {
                        termination = true;
                        break;
                    }
                }


                //Console.WriteLine("Text received : {0}", data);
                //byte[] msg = Encoding.ASCII.GetBytes(data);
                //handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                logger.Info("close the session");

                if (termination)
                {
                    logger.Info("Robot terminates");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            //Console.WriteLine(e.ToString());
        }

        //Console.WriteLine("\n Press any key to continue...");
        //Console.ReadKey();
    }

}