using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using rpa_robot.Formats;
using System.Threading;

namespace rpa_robot.Classes
{
    internal class Orchestrator
    {
        public static Queue<string> OrchestratorProcessQueue = new Queue<string>();
        public static WebSocket ws;
        public static bool Connected = false;
        public static void MakeAuthentication()
        {
            Log.Information("Service is tring to connect to the Orchestrator");
            Globals.LogsTxtBox.AppendText("Service is tring to connect to the Orchestrator\n");
            var RobotInormation = JsonConvert.SerializeObject(new RobotInfo
            {
                username = Globals.RobotUsername,
                password = Globals.RobotPassword
            });

            var content = new StringContent(RobotInormation,Encoding.UTF8, "application/json");
            while (!Connected)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // Send a POST request to the specified URL with the content and get the response
                        var response = client.PostAsync(Globals.AuthenticationEndPoint, content).Result;

                        // Read the response content as a string
                        var responseContent = response.Content.ReadAsStringAsync().Result;

                        // Get the status code
                        HttpStatusCode StatusCode = response.StatusCode;
                        if (StatusCode == HttpStatusCode.OK)
                        {
                            Log.Information("StatusCode 200[OK] is received!");
                            Globals.LogsTxtBox.AppendText("StatusCode 200[OK] is received!\n");

                            Token Tok = JsonConvert.DeserializeObject<Token>(responseContent);
                            ws = new WebSocket($"ws://35.242.197.187/rtlogs?token={Tok.token}");

                            Log.Information($"The Token: {Tok.token}");
                            Globals.LogsTxtBox.AppendText($"The Token: {Tok.token}\n");

                            ws.OnMessage += WebSocketISR;
                            ws.OnClose += WSOnClose;
                            ws.OnError += WSOnError;
                            ws.OnOpen += WSOnOpen;

                            Log.Information("Service is tring to connect to the Web socket!");
                            Globals.LogsTxtBox.AppendText("Service is tring to connect to the Web socket!\n");

                            ws.Connect();

                            Log.Information("Service connected to the Web socket!");
                            Globals.LogsTxtBox.AppendText("Service connected to the Web socket!\n");

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
                Thread.Sleep(500);
            }


        }

        private static void WSOnOpen(object sender, EventArgs e)
        {
            Log.Information("Websocket is open");
        }

        private static void WSOnError(object sender, ErrorEventArgs e)
        {
            Log.Information(e.Message);
        }

        private static void WSOnClose(object sender, CloseEventArgs e)
        {
            Log.Information(e.Reason);
        }

        private static void WebSocketISR(object sender, MessageEventArgs e)
        {
            lock (OrchestratorProcessQueue)
            {
                OrchestratorProcessQueue.Enqueue(e.Data);
            }
        }
    }
}
