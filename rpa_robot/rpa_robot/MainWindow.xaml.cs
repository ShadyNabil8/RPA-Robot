using Newtonsoft.Json;
using rpa_robot.Classes;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using rpa_robot.Formats;
using OpenQA.Selenium.DevTools;
using Serilog.Sinks.RichTextBox;
using Serilog.Core;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace rpa_robot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsDarkTheme { get; set; }
        public readonly PaletteHelper paletteHelper = new PaletteHelper();
        public static Logger UILogger;
        public MainWindow()
        {
            InitializeComponent();

            Initialization.AppInit();
            try
            {
                UILogger = new LoggerConfiguration()
                .WriteTo.RichTextBox(rtblogger, theme: LoggerTheme.Theme)
                .CreateLogger();
            }
            catch (Exception)
            {

                throw;
            }
            StateChanged += MainWindow_StateChanged;
            Globals.notifyIcon.Icon = new System.Drawing.Icon(Globals.ImagePath);
            Globals.notifyIcon.Text = "rpa_robot";
            Globals.notifyIcon.MouseClick += notifyIcon_MouseClick;

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            
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

        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();
            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            paletteHelper.SetTheme(theme);

        }

        private void exitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
    }
}
