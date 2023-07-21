using System.Threading;


namespace rpaService.Classes
{
    internal class Globals
    {
        public static string rootPath = @"C:\rboHive\";
        public static string usernamePath = @"C:\rboHive\info\username.txt";
        public static string passwordPath = @"C:\rboHive\info\password.txt";
        public static string userInfoPath = @"C:\rboHive\info";
        public static string userIdFile = @"C:\rboHive\info\uuid.txt";
        public static string robotAddressFile = @"C:\rboHive\info\ra.txt";
        public static string RobotUsername = string.Empty;
        public static string RobotPassword = string.Empty;
        public static string AuthenticationEndPoint = "http://34.155.103.216/api/robot/login/";
        public static string WebSocketCreationEndPoint = "ws://34.155.103.216/rtlogs?token=";
        public static string jobEndPoint = "ws://34.155.103.216/api/robots/connect";
        public static string uuid = string.Empty;
        public static string watcherPath = @"C:\rboHive\WfSource\";
        public static string LogFilePath = @"C:\rboHive\ServiceLog\ServiceLog.log";
        public static string LogDirectoryPath = @"C:\rboHive\ServiceLog\";
        public static string downloadPath = @"C:\rboHive\WfSource\Workflow.xaml";
    }
}
