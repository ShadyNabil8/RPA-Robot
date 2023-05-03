using System.Collections.Generic;
using System;
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

        //============================================== GLOGBALs ==============================================//
        public static string LogPath = @".\log\RobotLog.log";
        public static string WorkflowFilePath = "D:\\New folder\\CSE\\grad.Proj\\XAMLs\\ShowMB.xaml";
        public static string ImagePath = "D:\\New folder\\CSE\\grad.Proj\\img\\robot.ico";
        public static TextBox LogsTxtBox   = new TextBox();
        public static TextBox StatusTxtBox = new TextBox();
        public static System.Windows.Threading.Dispatcher uiDispatcher = Application.Current.Dispatcher;
        public static System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//
        public static string RobotUsername = "Shady";
        public static string RobotPassword = "Nabil";
        //============================================== GLOGBALs ==============================================//
        public static Queue<string> LogQueue = new Queue<string>();
    }
}
