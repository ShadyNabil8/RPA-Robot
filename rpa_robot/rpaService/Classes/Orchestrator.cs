using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using rpaService.Formats;
using WebSocketSharp;
using Serilog;
using System.Threading;
using System.Data.Common;

namespace rpaService.Classes
{
    internal class Orchestrator
    {
        public static Queue<string> OrchestratorProcessQueue = new Queue<string>();
        public static WebSocket ws;
        public static bool Transaction = true;
        public static bool Connected = false;
        public static void MakeAuthentication()
        {
            Log.Information("Service is tring to connect to the Orchestrator");
            //Globals.ServiceAsyncClientFromRobot.StartClient("Service is tring to connect to the Orchestrator");
            /*{
                var CMD = JsonConvert.SerializeObject(new ServiceRobotCMD
                {
                    Command = "print",
                    Data = "Service is tring to connect to the Orchestrator"
                });
                Globals.ServiceAsyncClientFromRobot.StartClient(CMD);
            }*/

            var RobotInormation = JsonConvert.SerializeObject(new RobotInfo {
                username = Globals.RobotUserName, password = Globals.RobotPassword
            });

            var content = new StringContent(RobotInormation, System.Text.Encoding.UTF8, "application/json");
            while (!Connected) 
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Send a POST request to the specified URL with the content and get the response
                        var response = client.PostAsync("http://35.242.197.187/api/robot/login/", content).Result;

                        // Read the response content as a string
                        var responseContent = response.Content.ReadAsStringAsync().Result;

                        // Get the status code
                        HttpStatusCode StatusCode = response.StatusCode;
                        if (StatusCode == HttpStatusCode.OK)
                        {
                            
                            Log.Information("StatusCode 200[OK] is received!");

                            Token Tok = JsonConvert.DeserializeObject<Token>(responseContent);
                            ws = new WebSocket($"ws://35.242.197.187/rtlogs?token={Tok.token}");
                            Log.Information($"The Token: {Tok.token}");

                            ws.OnMessage += Handler.WebSocketISR;
                            Log.Information("Service is tring to connect to the Web socket!");

                            ws.Connect();
                            Log.Information("Service connected to the Web socket!");
                            Connected = true;



                        }
                        else
                        {
                            Message msg = JsonConvert.DeserializeObject<Message>(responseContent);
                        }
                    }
                }
                catch (Exception)
                {

                    Log.Information("Service cannot connect to the orchestrator!");

                }
            }
            
            
        }

        
    }
    
}
