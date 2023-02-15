using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rpaSlayerRobot
{
    public partial class rpaSlayerRobotForm : Form
    {
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


            //Add log configuration here
        }

        private void tcpWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpSocket.StartServer();
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
    }
}
