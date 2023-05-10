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
        public static BackgroundWorker  Robot   = new BackgroundWorker();
        public static FileSystemWatcher watcher = new FileSystemWatcher();
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//
        public static string LogPath = @".\log\RobotLog.log";
        public static string WorkflowFilePath = @".\WfDest\Workflow.xaml";
        public static string ImagePath = @"D:\New folder\CSE\grad.Proj\img\robot.ico";
        public static string watcherPath = @".\WfSource\";
        public static string sourceFilePath = @".\WfSource\Workflow.xaml";
        public static string destinationFilePath = @".\WfDest\Workflow.xaml";
        public static string serviceExePath = @"D:\New folder\CSE\grad.Proj\rpa_robot\rpaService\bin\Debug\rpaService.exe";
        public static TextBox LogsTxtBox   = new TextBox();
        public static TextBox StatusTxtBox = new TextBox();
        public static System.Windows.Threading.Dispatcher uiDispatcher = Application.Current.Dispatcher;
        public static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//

    }
}
