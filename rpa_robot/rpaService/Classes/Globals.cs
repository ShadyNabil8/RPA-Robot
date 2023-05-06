﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Globals
    {
        public static AsynchronousSocketListener ServiceAsyncListenerFromRobot = new AsynchronousSocketListener();
        public static AsynchronousClient ServiceAsyncClientFromRobot           = new AsynchronousClient();
        public static Thread ServiceAsyncListenerFromRobotThread               = new Thread(Handler.ServiceAsyncListenerFromRobotFun);
        public static Thread ServiceAsyncListenerFromRobotThreadHnadler        = new Thread(Handler.ServiceAsyncListenerFromRobotThreadHnadlerFun);


        public static string LogPath = @"D:\New folder\CSE\grad.Proj\logs\ServiceLog.log";

        public static string RobotUserName = "we";
        public static string RobotPassword = "we";
        public static string AuthenticationEndPoint = "http://35.242.197.187/api/robot/login/";

    }

}