using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace rpa_robot.Classes
{

    internal class Globals
    {
        //============================================== GLOGBALs ==============================================//
        //public static AsynchronousSocketListener RobotAsyncListenerFromService       = new AsynchronousSocketListener();
        //public static AsynchronousClient         RobotAsyncClientFromService         = new AsynchronousClient();
        //public static BackgroundWorker SharedFolderWorker = new BackgroundWorker();    
        public static BackgroundWorker LogWorker = new BackgroundWorker();
        public static FileSystemWatcher watcher  = new FileSystemWatcher();
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//
        public static string LogPath = @".\log\RobotLog.log";
        public static string WorkflowFilePath = @"D:\MY TESTS\SharedFloder\SharedFolder\dst\Workflow.xaml";
        //public static string WorkflowFilePath = @".\WfDest\Workflow.xaml";
        public static string ImagePath = @"D:\New folder\CSE\grad.Proj\img\robot.ico";
        public static string watcherPath = @"D:\MY TESTS\SharedFloder\SharedFolder\src\";
        //public static string watcherPath = @".\WfSource\";
        public static string sourceFilePath = @"D:\MY TESTS\SharedFloder\SharedFolder\src\Workflow.xaml";
        //public static string sourceFilePath = @".\WfSource\Workflow.xaml";
        //public static string destinationFilePath = @".\WfDest\Workflow.xaml";
        public static string destinationFilePath = @"D:\MY TESTS\SharedFloder\SharedFolder\dst\Workflow.xaml";
        public static TextBox LogsTxtBox   = new TextBox();
        public static TextBox StatusTxtBox = new TextBox();
        public static System.Windows.Threading.Dispatcher uiDispatcher = Application.Current.Dispatcher;
        public static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//
        public static string RobotUsername = "we";
        public static string RobotPassword = "we";
        public static string AuthenticationEndPoint = "http://35.242.197.187/api/robot/login/";
        public static string WebSocketCreationEndPoint = "ws://35.242.197.187/rtlogs?token=";
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//

    }
}
