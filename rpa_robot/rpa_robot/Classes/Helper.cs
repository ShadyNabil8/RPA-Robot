using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using rpa_robot.Formats;
using Serilog;
using System.Threading;
using System.Net.NetworkInformation;
using System.Windows;
using System.Activities.Tracking;
using System.Globalization;
using Serilog.Core;

namespace rpa_robot.Classes
{
    internal class Helper
    {
        public static bool IsServiceInstalled()
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName.Equals(Globals.serviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        public static string ReadUserID()
        {
            return Globals.uuid;
        }
        public static string ReadRobotAd()
        {
            string macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            return macAddr;
        }
        public static bool CheckForFolders()
        {
            bool allOk = true;
            try
            {
                if (!Directory.Exists(Globals.rootPath))
                {
                    Directory.CreateDirectory(Globals.rootPath);
                    Log.Information("rootPath created");
                }
            }
            catch (Exception ex)
            {

                Log.Error($"Filed to check or create the rootPath: {ex.Message}");
                allOk = false;
            }
            try
            {
                if (!Directory.Exists(Globals.userInfoPath))
                {
                    Directory.CreateDirectory(Globals.userInfoPath);
                    Log.Information("Information folder created");
                }
            }
            catch (Exception ex)
            {

                Log.Error($"Filed to check or create the folder containing user information: {ex.Message}");
                allOk = false;
            }
            try
            {
                if (!File.Exists(Globals.usernamePath))
                {
                    using (File.Create(Globals.usernamePath))
                    {
                        Log.Information("username file created");
                    }
                }
            }
            catch (Exception ex)
            {

                Log.Error($"Filed to check or create the username file: {ex.Message}");
                allOk = false;
            }
            try
            {
                if (!File.Exists(Globals.passwordPath))
                {
                    using (File.Create(Globals.passwordPath))
                    {
                        Log.Information("Password file created");
                    }
                }
            }
            catch (Exception ex)
            {

                Log.Error($"Filed to check or create the password file: {ex.Message}");
                allOk = false;
            }
            return allOk;
        }
        public static bool CreateFiles(string username, string password)
        {
            bool allOk = true;
            try
            {
                File.WriteAllText(Globals.usernamePath, username);
                Log.Information("username is stored");
            }
            catch (Exception ex)
            {

                Log.Error($"Failed to store the username: {ex.Message}");
                allOk = false;
            }
            try
            {
                File.WriteAllText(Globals.passwordPath, password);
                Log.Information("Password is stored");
            }
            catch (Exception ex)
            {

                Log.Error($"Failed to store the Password: {ex.Message}");
                allOk = false;
            }
            return allOk;
        }
        public static void CpyWorkFlow()
        {
            try
            {
                // Copy the file
                if (File.Exists(Globals.sourceFilePath))
                {
                    File.Copy(Globals.sourceFilePath, Globals.destinationFilePath, true);
                    Log.Information("File copied successfully.");
                }
                //File.Copy(Globals.sourceFilePath, Globals.destinationFilePath, true);

            }
            catch (IOException er)
            {
                Log.Error($"An error occurred while copying the file: {er.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
            }
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
                Log.Error($"An error occurred while deleting the file: {er.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
            }
        }
        public static async Task<bool> verify(string username, string password)
        {

            var robotInformation = JsonConvert.SerializeObject(new RobotInfo
            {
                username = username,
                password = password,
            });
            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    // Create the content for the POST request with the serialized robot information
                    var content = new StringContent(robotInformation, Encoding.UTF8, "application/json");

                    // Send a POST request to the specified URL with the content and get the response
                    response = await client.PostAsync(Globals.AuthenticationEndPoint, content);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Log.Error("An error occurred during the POST request: Internet issue" + ex.Message);
                    // Handle the exception or rethrow it to trigger a retry
                    throw;
                }

                // Get the status code
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                else
                {
                    // Authentication unsuccessful (status code other than 200 [OK])
                    // Deserialize the response content into a Message object
                    return false;
                }
            }
            return false;
        }
        public static void SendToService(string log) 
        {
            AsynchronousClient.StartClient(log);
        }
        public static void OnLoggedIn(string cardinalities)
        {
            //SendToService(cardinalities);
            MainWindow main = new MainWindow();
            main.Show();
        }
        public static void PrintOnUI(string data)
        {
            MainWindow.UILogger.Information(data);
        }
        public static string GetTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            DateTime cairoTime = TimeZoneInfo.ConvertTime(utcTime, cairoTimeZone);
            string cairoTimeString = cairoTime.ToString("o", CultureInfo.InvariantCulture);
            return cairoTimeString;
        }
    }
}
