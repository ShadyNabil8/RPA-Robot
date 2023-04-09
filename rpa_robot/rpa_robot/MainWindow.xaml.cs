using rpa_robot.Classes;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;


namespace rpa_robot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            Service.Initialize();
            
            //================== LOGGING =================
            Log.Logger = new LoggerConfiguration()        //
                .WriteTo.File(Globals.LogPath)            //
                .CreateLogger();                          //

            Grid.SetRow(Globals.LogsTxtBox, 1);
            Grid.SetRow(Globals.StatusTxtBox, 1);
            logs.Children.Add(Globals.LogsTxtBox);
            status.Children.Add(Globals.StatusTxtBox);
            //RobotReportList.ItemsSource = Globals.List; //
            //================== LOGGING =================  

            //=======================================================================================
            Globals.RobotAsyncListenerFromServiceWorker.DoWork += Handler.RobotAsyncListenerFromServiceFun;   
            Globals.Robot.DoWork                               += Handler.RobotFun;                          
            Globals.RobotAsyncListenerFromServiceWorker.RunWorkerAsync();                            
            Globals.Robot.RunWorkerAsync();                                                          
            //=======================================================================================
        }

        private void OnStartServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.Start();  
        }

        private void OnStopServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.Stop();
            //Globals.RobotAsyncClientFromService.SendToSocket("Welcome");
        }

        private void OnInstallServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.InstallAndStart();
        }

        private void OnUninstallServiceButtonClick(object sender, RoutedEventArgs e)
        {
            Service.UninstallAndStop();
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Log.Information(Info.MAIN_WINDOW_IS_COLSED);
            //Service.Stop(); 
        }
        public static void AppendTxt(string text)
        {
            Globals.LogsTxtBox.AppendText(text);
        }

    }
}
