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
        public static string rootPath = @"C:\rboHive\";
        public static string usernamePath = @"C:\rboHive\info\username.txt";
        public static string passwordPath = @"C:\rboHive\info\password.txt";
        public static string userInfoPath = @"C:\rboHive\info";
        public static string userIdFile = @"C:\rboHive\info\uuid.txt";
        public static string robotAddressFile = @"C:\rboHive\info\ra.txt";
        public static string serviceName = "rpaService";
        public static string uuid = string.Empty;
        public static string LogFilePath = @"C:\rboHive\RobotLog\RobotLog.log";
        public static string LogDirectoryPath = @"C:\rboHive\RobotLog\";
        //public static string WorkflowFilePath = @"C:\Users\shady\Desktop\Assemblies\Workflow.xaml";
        public static string WorkflowFilePath = @"C:\rboHive\WfDest\Workflow.xaml";
        //public static string ImagePath = @"D:\New folder\CSE\grad.Proj\img\robot.ico";
        public static string ImagePath = @".\logo.ico";
        //public static string watcherPath = @".\WfSource\";
        public static string watcherPath = @"C:\rboHive\WfSource\";
        public static string destinationPath = @"C:\rboHive\WfDest\";
        //public static string sourceFilePath = @".\WfSource\Workflow.xaml";
        public static string sourceFilePath = @"C:\rboHive\WfSource\Workflow.xaml";
        //public static string destinationFilePath = @".\WfDest\Workflow.xaml";
        public static string destinationFilePath = @"C:\rboHive\WfDest\Workflow.xaml";
        public static string serviceExePath = @"D:\New folder\CSE\grad.Proj\rpa_robot\rpaService\bin\Debug\rpaService.exe";
        public static string AuthenticationEndPoint = "http://34.155.103.216/api/robot/login/";
        public static System.Windows.Threading.Dispatcher uiDispatcher = Application.Current.Dispatcher;
        public static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();


    }
}
