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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rpaSlayerRobot
{
    public partial class rpaSlayerRobotForm : Form
    {
        private AsynchronousSocketListener serviceAsServer;
        private AsynchronousClient serviceAsClient;

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

            serviceAsServer = new AsynchronousSocketListener();
            serviceAsClient = new AsynchronousClient();

            tcpWorker.RunWorkerAsync();
        }

        private void tcpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            serviceAsServer.StartListening();

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
            if (w1.IsBusy)
            {
                MessageBox.Show("Robot In Progress");
            }
            else
            {
               w1.RunWorkerAsync();

            }

        }

        private void w1_DoWork(object sender, DoWorkEventArgs e)
        {
            serviceAsClient.StartClient();
        }

    }
}
