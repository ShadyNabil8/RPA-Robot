using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace rpa_robot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Login loginWindow = new Login();
            loginWindow.ShowDialog();

            MainWindow mainWindow = new MainWindow();
            //mainWindow.Visibility = Visibility.Visible;
            mainWindow.Activate();

        }
    }
}
