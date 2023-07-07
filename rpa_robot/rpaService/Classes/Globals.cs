using System.Threading;


namespace rpaService.Classes
{
    internal class Globals
    {
        //============================================== GLOGBALs ==============================================//
        public static Thread ListenerFromRobot = new Thread(Handler.ListenerFromRobothHandler);
        public static Thread LoggingProcess    = new Thread(Handler.LoggingProcessHandler);
        //============================================== GLOGBALs ==============================================//

        //============================================== GLOGBALs ==============================================//
        public static string RobotUsername = "kz";
        public static string RobotPassword = "pw";
        public static string AuthenticationEndPoint = "http://34.155.103.216/api/robot/login/";
        public static string WebSocketCreationEndPoint = "ws://34.155.103.216/rtlogs?token=";
        public static string watcherSrcPath = @"D:\New folder\CSE\grad.Proj\rpa_robot\rpa_robot\bin\Debug\WfSource\workflow.xaml";
        public static string LogFilePath = @"C:\rboHive\ServiceLog\ServiceLog.log";
        public static string LogDirectoryPath = @"C:\rboHive\ServiceLog\";
        public static string watcherPath = @"C:\rboHive\WfSource\";
        //============================================== GLOGBALs ==============================================//

    }

}
