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
        public static string RobotUsername = "ludaman";
        public static string RobotPassword = "pw";
        public static string AuthenticationEndPoint = "http://34.155.103.216/api/robot/login/";
        public static string WebSocketCreationEndPoint = "ws://34.155.103.216/rtlogs?token=";
        public static string jobEndPoint = "ws://34.155.103.216/api/robots/connect";
        public static string uuid = null;
        public static string watcherPath = @"C:\rboHive\WfSource\";
        public static string LogFilePath = @"C:\rboHive\ServiceLog\ServiceLog.log";
        public static string LogDirectoryPath = @"C:\rboHive\ServiceLog\";
        public static string downloadPath = @"C:\rboHive\WfSource\Workflow.xaml";
        //============================================== GLOGBALs ==============================================//

    }

}
