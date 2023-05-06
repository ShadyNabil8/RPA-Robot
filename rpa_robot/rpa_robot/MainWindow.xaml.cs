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
            //Service.Initialize();

            //================== LOGGING AND NOTIFICATION =================
            Log.Logger = new LoggerConfiguration()        //
                .WriteTo.File(Globals.LogPath)            //
                .CreateLogger();                          //

            Grid.SetRow(Globals.LogsTxtBox, 1);
            Grid.SetRow(Globals.StatusTxtBox, 1);
            logs.Children.Add(Globals.LogsTxtBox);
            status.Children.Add(Globals.StatusTxtBox);

            StateChanged += MainWindow_StateChanged;
            Globals.notifyIcon.Icon = new System.Drawing.Icon(Globals.ImagePath);
            Globals.notifyIcon.Text = "rpa_robot";
            Globals.notifyIcon.MouseClick += notifyIcon_MouseClick;
            //================== LOGGING =================  

            //=======================================================================================
            //Globals.SharedFolderWorker.DoWork += Handler.SharedFolderProcess;
            Globals.LogWorker.DoWork += Handler.LoggingProcess;
            //Globals.SharedFolderWorker.RunWorkerAsync();
            Globals.LogWorker.RunWorkerAsync();
            Globals.watcher.Path = Globals.watcherPath;
            Globals.watcher.Created += Handler.OnFileCreated;
            // Enable the watcher
            Globals.watcher.EnableRaisingEvents = true;
            //=======================================================================================
            Orchestrator.MakeAuthentication();
        }

        
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Log.Information(Info.MAIN_WINDOW_IS_COLSED);
            //Service.Stop(); 
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                // Hide the main window and show the NotifyIcon
                Visibility = Visibility.Hidden;
                Globals.notifyIcon.Visible = true;
            }
            else
            {
                // Show the main window and hide the NotifyIcon
                Visibility = Visibility.Visible;
                Globals.notifyIcon.Visible = false;
            }
        }
        private void notifyIcon_MouseClick(object sender, EventArgs e)
        {
            // Show or hide the main window when the user clicks on the NotifyIcon
            Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
