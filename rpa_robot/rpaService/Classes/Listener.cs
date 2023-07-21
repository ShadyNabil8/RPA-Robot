using Newtonsoft.Json;
using rpaService.Formats;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Listener
    {
        public static void Listen() 
        {
            AsynchronousSocketListener.StartListening();
        }
        public static void DataReceivedHandler(string data)
        {
            RobotMsg robotMsg = null;
            try
            {
                robotMsg = JsonConvert.DeserializeObject<RobotMsg>(data);
            }
            catch (Exception)
            {

                Log.Error("Error in Deserializing the robot msg!");
            }
            if (!robotMsg.eventType.Equals("login"))
            {
                try
                {
                    if ((LogSender.LogSenderSocket != null) && (LogSender.LogSenderSocket.IsAlive))
                    {
                        LogSender.LogSenderSocket.SendAsync(data, (completed) =>
                        {
                            if (completed)
                            {
                                // Make something
                            }
                            else
                            {
                                Log.Information("Failed to send data.");
                            }
                        });

                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occurred during the SendAsync : {ex}");
                }
            }
            else
            {
                try
                {
                    RobotInfo loginData = JsonConvert.DeserializeObject<RobotInfo>(robotMsg.payload);
                    Globals.RobotUsername = loginData.username;
                    Globals.RobotPassword = loginData.password;
                }
                catch (Exception)
                {
                    Log.Error("Error in Deserializing the login data!");
                }
                Reconnector.Reconnect();
            }
        }
    }
}
