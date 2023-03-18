using rpa_robot.Classes;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
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
        //======================= GLOGBALs =======================//
        private AsynchronousSocketListener RobotAsyncListenerFromService;
        private AsynchronousClient         RobotAsyncClientFromService;
        private BackgroundWorker           RobotAsyncListenerFromServiceWorker = new BackgroundWorker();
        private BackgroundWorker           Robot = new BackgroundWorker();
        //======================= GLOGBALs =======================//
        public MainWindow()
        {
            InitializeComponent();
            Service.Initialize();

            RobotAsyncListenerFromServiceWorker.DoWork += RobotAsyncListenerFromServiceFun;
            Robot.DoWork += RobotFun;

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\New folder\CSE\grad.Proj\logs\RobotLog.log")
                .CreateLogger();

            RobotAsyncListenerFromService = new AsynchronousSocketListener();
            RobotAsyncClientFromService = new AsynchronousClient();
            RobotAsyncListenerFromServiceWorker.RunWorkerAsync();
            Robot.RunWorkerAsync();
        }

        private void RobotAsyncListenerFromServiceFun(object sender, DoWorkEventArgs e)
        {
            RobotAsyncListenerFromService.StartListening();
        }
        private void RobotFun(object sender, DoWorkEventArgs e)
        {

            while (true)
            {
                if (RobotAsyncListenerFromService.ProcessQueue.Count > 0)
                {
                    Log.Information(RobotAsyncListenerFromService.ProcessQueue.Dequeue() + "FROM Q");
                }
                else
                {
                    //== THIS LINE IS WRITTEN TO AVOID THE OVEDHEAD DUE TO THE WHILE LOOP, LOOPING ON NOTHING ==//
                    Thread.Sleep(1000);
                   
                }
            }
        }
        
        private void OnStartServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.Start();  
        }

        private void OnStopServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.Stop();
        }

        private void OnInstallServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.InstallAndStart();
        }

        private void OnUninstallServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.UninstallAndStop();
        }
    }
}
