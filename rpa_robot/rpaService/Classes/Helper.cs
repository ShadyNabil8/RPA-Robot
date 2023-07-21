using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using rpaService.Formats;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Helper
    {
        public static void checkLogFileSize()
        {
            FileInfo fileInfo = new FileInfo(Globals.LogFilePath);
            long fileSizeInBytes = fileInfo.Length;

            const long bytesInMegabyte = 1024 * 1024;
            long fileSizeInMegabytes = fileSizeInBytes / bytesInMegabyte;

            if (fileSizeInMegabytes > 2)
            {
                File.Delete(Globals.LogFilePath);
                Log.Information("Log File deleted successfully.");
            }
        }
        public static void SendUuid(string uuid)
        {
            string robotAddress =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            var robotInformation = JsonConvert.SerializeObject(new RobotMsg
            {
                eventType = "uuid",
                payload = uuid
            });
            AsynchronousClient.StartClient(robotInformation);
        }
        public static bool CheckForValidCredentials()
        {
            return (!string.IsNullOrEmpty(Globals.RobotUsername) && !string.IsNullOrEmpty(Globals.RobotPassword)); 
        }
        public static bool CheckForValidToken(Token token)
        {
            return !(token == null);
        }
        public static bool CheckForValidUuid()
        {
            return !(Globals.uuid == string.Empty);
        }
        public static void DeleteeWorkFlow(string Path)
        {
            try
            {
                // Delete the file
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    Log.Information("File deleted successfully.");
                }
                //File.Delete(Path);    
            }
            catch (IOException er)
            {
                Log.Information($"An error occurred while deleting the file: {er.Message}");
            }
            catch (Exception ex)
            {
                Log.Information($"An error occurred: {ex.Message}");
            }
        }
        public static void DownloadPackage(string address) 
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Task.Run(() =>
                    {
                        Helper.DeleteeWorkFlow(Globals.downloadPath);
                        webClient.DownloadFile(address, Globals.downloadPath);
                        Log.Information("File downloaded successfully.");
                    });

                }
                catch (Exception ex)
                {
                    Log.Information("jobws: An error occurred while downloading the file: " + ex.Message);
                }
            }
        }
    }
}
