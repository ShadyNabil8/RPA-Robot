using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace rpa_robot.Classes
{

    internal class Globals
    {
        //============================================== GLOGBALs ==============================================//
        public static AsynchronousSocketListener RobotAsyncListenerFromService       = new AsynchronousSocketListener();
        public static AsynchronousClient         RobotAsyncClientFromService         = new AsynchronousClient();
        public static BackgroundWorker           RobotAsyncListenerFromServiceWorker = new BackgroundWorker();    
        public static BackgroundWorker           Robot                               = new BackgroundWorker();
        //============================================== GLOGBALs ==============================================//
        

        public static string LogPath = @"D:\New folder\CSE\grad.Proj\logs\RobotLog.log";
        public static string WorkflowFilePath = "D:\\New folder\\CSE\\grad.Proj\\XAMLs\\Workflow3.xaml";
        public static ObservableCollection<RobotReport> List = new ObservableCollection<RobotReport>();
        public static void AddToList(string msg, string type) 
        {
            Globals.List.Add(new RobotReport(msg, type));
        }

        public static TextBox LogsTxtBox   = new TextBox();
        public static TextBox StatusTxtBox = new TextBox();
        public static System.Windows.Threading.Dispatcher uiDispatcher = Application.Current.Dispatcher;


    }
}
