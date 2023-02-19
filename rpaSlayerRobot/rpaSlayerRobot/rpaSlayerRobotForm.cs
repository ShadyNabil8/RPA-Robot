using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rpaSlayerRobot
{
    public partial class rpaSlayerRobotForm : Form
    {
        private TcpSocket service;

        public rpaSlayerRobotForm()
        {
            InitializeComponent();
        }

        private void rpaSlayerRobotForm_Load(object sender, EventArgs e)
        {
            // Initialize the notofications
            notifyIcon1.BalloonTipTitle = "rpaSlayerRobot";
            notifyIcon1.BalloonTipText = "Robot has started";
            notifyIcon1.Text = "rpaSlayerRobot";


            //Add log configuration 
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\RobotLog.log")
                .CreateLogger();

            service=new TcpSocket();

        }

        private void tcpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                service.ReciveByTCP(service.RobotListeningIP, 9001);
                Log.Information("ROBOT IS READY TO LISTEN FROM SERVICE!");
                while (true)
                {
                    Log.Information("ROBOT IS LISTENING FROM SERVICE!");
                    Socket handler = service.RobotListener.Accept();
                    Log.Information("CONNEDTED TO SERVICE!");
                    try
                    {
                        byte[] buffer = new byte[255];
                        int rec = handler.Receive(buffer);
                        if (rec > 0)
                        {
                            Array.Resize(ref buffer, rec);
                            Log.Information("ROBOT RECEIVED : " + Encoding.ASCII.GetString(buffer, 0, rec));
                        }
                        else
                        {
                            throw new SocketException();
                        }
                    }
                    catch
                    {
                        Log.Error("CONNECTION WITH SERVICE FAILED");
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch
            {
                Log.Error("ROBOT CANNOT CONNECT TO SERVICE");
            }

        }



        private void button1_Click(object sender, EventArgs e)
        {
            // Call the worker to begin listining for the incomming calles
            if (tcpWorker.IsBusy)
            {
                MessageBox.Show("Robot In Progress");
            }
            else
            {
                tcpWorker.RunWorkerAsync();

            }
        }

        private void notifyIcon1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            // Open the form
            Show();

            // Form in the normal size
            WindowState = FormWindowState.Normal;

            // Not to see the notification icon whzile form is visible
            notifyIcon1.Visible = false;

        }

        private void rpaSlayerRobotForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;

                // 1 sec
                notifyIcon1.ShowBalloonTip(1000);
            }
            else if (WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TcpSocket Service = new TcpSocket();
            Service.SendByTCP(Service.ServiceListeningIP, 9000, "I AM ROBOT");
        }
    }
}
