using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using rpa_robot.Classes;
using rpa_robot.Formats;

namespace rpa_robot
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public bool IsDarkTheme { get; set; }
        public readonly PaletteHelper paletteHelper = new PaletteHelper();
        public Login()
        {
            InitializeComponent();
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

        private async void loginbtn_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtUsername.Text;
            bool check = await Helper.verify(username, password);
            if (check)
            {
                var robotInformation = JsonConvert.SerializeObject(new Data
                {
                    eventType = "login",
                    payload = JsonConvert.SerializeObject(new RobotInfo
                    {
                        username = username,
                        password = password
                    })
                });
                Helper.OnLoggedIn(robotInformation);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
            
        }
    }
}
