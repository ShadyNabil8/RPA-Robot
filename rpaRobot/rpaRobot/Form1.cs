using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;

namespace rpaRobot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the notofications
            notifyIcon1.BalloonTipTitle = "rpaRobot";
            notifyIcon1.BalloonTipText  = "Robot has started";
            notifyIcon1.Text            = "rpaRobot";
            
            // Initialize the log system
            LogsInit();
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
            var Appender          = new FileAppender();
            Appender.Name         = "RobotApppender";
            Appender.Layout       = Layout;
            Appender.Threshold    = Level.All;
            Appender.AppendToFile = true;
            Appender.File         = "E:\\CSE\\4th comp\\grad.Proj\\logs\\RobotLog.log";
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
            ILog logger = LogManager.GetLogger(typeof(Form1));

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

        private void button1_Click(object sender, EventArgs e)
        {
            // Call the worker to begin listining for the incomming calles
            if (Worker.IsBusy)
            {
                MessageBox.Show("Robot In Progress");
            }
            else 
            {
                Worker.RunWorkerAsync();
                RobotStatus.Text = "InProgress!!";
            }
            
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Open the form
            this.Show();

            // Form in the normal size
            WindowState = FormWindowState.Normal;

            // Not to see the notification icon while form is visible
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;

                // 1 sec
                notifyIcon1.ShowBalloonTip(1000);
            }
            else if(WindowState == FormWindowState.Normal) 
            {
                notifyIcon1.Visible = false;

            }
        }

        private void Worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            StartServer();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}