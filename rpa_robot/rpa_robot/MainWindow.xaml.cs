using rpa_robot.Classes;
using Serilog;
using System;
using System.IO;
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
            try
            {
                CreateWorkflowFolders();
            }
            catch (Exception ex)
            {
                //Log.Information("Error creating folder: " + ex.Message);
            }
            try
            {
                Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Globals.LogFilePath)
                .CreateLogger();
            }
            catch (Exception)
            { throw; }


            Grid.SetRow(Globals.LogsTxtBox, 1);
            Grid.SetRow(Globals.StatusTxtBox, 1);
            logs.Children.Add(Globals.LogsTxtBox);
            status.Children.Add(Globals.StatusTxtBox);

            StateChanged += MainWindow_StateChanged;
            Globals.notifyIcon.Icon = new System.Drawing.Icon(Globals.ImagePath);
            Globals.notifyIcon.Text = "rpa_robot";
            Globals.notifyIcon.MouseClick += notifyIcon_MouseClick;

            //=======================================================================================
            Globals.watcher.Path = Globals.watcherPath;
            Globals.watcher.Created += Handler.OnFileCreated;
            //Globals.watcher.
            // Enable the watcher
            Globals.watcher.EnableRaisingEvents = true;

            Globals.Robot.DoWork += Handler.RobotProcess;
            Globals.Robot.RunWorkerAsync();
            //=======================================================================================

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Log.Information(Info.MAIN_WINDOW_IS_COLSED);
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
        private void CreateWorkflowFolders()
        {
            if (!Directory.Exists(Globals.watcherPath))
            {
                Directory.CreateDirectory(Globals.watcherPath);
            }
            if (!Directory.Exists(Globals.destinationPath))
            {
                Directory.CreateDirectory(Globals.destinationPath);
            }
            if (!Directory.Exists(Globals.LogDirectoryPath))
            {
                Directory.CreateDirectory(Globals.LogDirectoryPath);
            }
            if (!File.Exists(Globals.LogFilePath))
            {
                using (File.Create(Globals.LogFilePath))
                {
                    //Log.Information("Log file created.");
                }
            }
            //Log.Information("Folder created successfully.");
        }
    }
}
