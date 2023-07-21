using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using rpaService.Formats;
using WebSocketSharp;
using Serilog;
using System.Threading.Tasks;
using Polly;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using System.IO;

namespace rpaService.Classes
{
    internal class LogSender
    {
        public static WebSocket LogSenderSocket = null;
        public static async Task MakeAuthenticationAsync()
        {
            Token token = null;
            try
            {
                if (Helper.CheckForValidCredentials())
                {
                    token = await GetToken();
                    Globals.uuid = token.userID; 
                }
                else
                {
                    Log.Error("Invalid credentials!");
                }

            }
            catch (Exception)
            {
                Log.Error("Error in getting token");
            }
            try
            {
                if (Helper.CheckForValidToken(token))
                {
                    await ConnectToWs(token);
                }
                else
                {
                    Log.Error("Invalid token!");
                }

            }
            catch (Exception)
            {
                Log.Error("Error in connecting to LogSenderSocket");
            }
        }

        private static void WSOnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Log.Error(e.Message);
        }

        private static void WSOnClose(object sender, CloseEventArgs e)
        {
            Log.Error($"LogSenderSocket is closed: : {e.Reason}");
        }

        private static void WebSocketISR(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                Log.Information("Ping!");
                Reconnector.loggingWspingReceived = true;
            }
            else
            {
                Log.Information("ACK!");
            }
        }


        static async Task<Token> GetToken()
        {
            Token token = null;
            var robotInformation = JsonConvert.SerializeObject(new RobotInfo
            {
                username = Globals.RobotUsername,
                password = Globals.RobotPassword,
            });
            Log.Information($"The username: {Globals.RobotUsername}");
            Log.Information($"The password: {Globals.RobotPassword}");
            Log.Information("Trying to connect to the Orchestrator");
            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    var content = new StringContent(robotInformation, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(Globals.AuthenticationEndPoint, content);
                }
                catch (Exception ex)
                {
                    Log.Error("An error occurred during sending username and password" + ex.Message);
                    throw;
                }
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        token = JsonConvert.DeserializeObject<Token>(responseContent);
                    }
                    catch (Exception)
                    {
                        Log.Error("Orchws: Error in Deserializing the token");
                    }
                    Log.Information($"userID: {token.userID}");
                    Helper.SendUuid(token.userID);
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Message msg = JsonConvert.DeserializeObject<Message>(responseContent);
                    Log.Information(msg.message);
                }
            }
            return token;
        }
        static async Task ConnectToWs(Token token)
        {
            Log.Information("Service is trying to connect to LogSenderSocket!");
            LogSenderSocket = new WebSocket($"{Globals.WebSocketCreationEndPoint}{token.token}");
            LogSenderSocket.OnMessage += WebSocketISR;
            LogSenderSocket.OnClose += WSOnClose;
            LogSenderSocket.OnError += WSOnError;
            LogSenderSocket.EmitOnPing = true;
            var connectionTaskCompletionSource = new TaskCompletionSource<bool>();
            LogSenderSocket.OnOpen += (sender, e) =>
            {
                connectionTaskCompletionSource.SetResult(true);
                Log.Information("Connected to LogSenderSocket!");
            };
            await Task.Run(() => LogSenderSocket.Connect());
            await connectionTaskCompletionSource.Task;
        }
    }

}
