using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Globals
    {
        //public static AsynchronousSocketListener ServiceAsyncListenerFromRobot = new AsynchronousSocketListener();
        //public static AsynchronousClient ServiceAsyncClientFromRobot           = new AsynchronousClient();
        public static Thread ListenerFromRobot = new Thread(Handler.ListenerFromRobothHandler);
        public static Thread LoggingProcess    = new Thread(Handler.LoggingProcessHandler);



        public static string LogPath = @"C:\Users\shady\Desktop\log\ServiceLog.log";
        //============================================== GLOGBALs ==============================================//
        public static string RobotUsername = "we";
        public static string RobotPassword = "we";
        public static string AuthenticationEndPoint = "http://35.242.197.187/api/robot/login/";
        public static string WebSocketCreationEndPoint = "ws://35.242.197.187/rtlogs?token=";
        //============================================== GLOGBALs ==============================================//

    }

}
