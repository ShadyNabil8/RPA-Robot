using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace rpa_robot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AsynchronousSocketListener serviceAsServer;
        private AsynchronousClient serviceAsClient;
        BackgroundWorker server = new BackgroundWorker();
        BackgroundWorker client = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            server.DoWork += serverJob;
            client.DoWork += clientJob;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\RobotLog.log")
                .CreateLogger();

            serviceAsServer = new AsynchronousSocketListener();
            serviceAsClient = new AsynchronousClient();
            server.RunWorkerAsync();
            client.RunWorkerAsync();
        }

        private void serverJob(object sender, DoWorkEventArgs e)
        {
            serviceAsServer.StartListening();
        }
        private void clientJob(object sender, DoWorkEventArgs e)
        {
            //serviceAsClient.StartClient();
            while (true)
            {
                if (serviceAsServer.ProcessQueue.Count > 0)
                {
                    Log.Information(serviceAsServer.ProcessQueue.Dequeue() + "FROM q");
                }
                else 
                {
                    
                }
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            //if (client.IsBusy)
            //{
            //    MessageBox.Show("Robot In Progress");
            //}
            //else
            //{
            //    client.RunWorkerAsync();

            //}
            //Handler.RunWorkFlow();
            serviceAsClient.StartClient();
        }
    }
}
