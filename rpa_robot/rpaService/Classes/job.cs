using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using WebSocketSharp;
using rpaService.Formats;
using Newtonsoft.Json.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft;
using System.Threading;

namespace rpaService.Classes
{
    internal class job
    {
        public static WebSocket jobws = null; /* workflow ws */
        private static int MaxRetryCount = 10;
        public static async void JobWsInit()
        {
            var jobWsretryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: MaxRetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        // Log the retry attempt
                        Log.Information($"Job Ws:Retry attempt {retryCount}. Retrying in {timespan.TotalSeconds} seconds.");

                    });
            await jobWsretryPolicy.ExecuteAsync(async () =>
            {
                //jobws = new WebSocket($"{Globals.WebSocketCreationEndPoint}{token.token}");

                // Subscribe to WebSocket events
                //jobws.OnMessage += jobWebSocketISR;
                //jobws.OnClose += jobWSOnCloseAsync;
                //jobws.OnError += jobWSOnError;
                //jobws.EmitOnPing = true;

                // Create a task completion source to track the completion of the connection
                //var jobWsconnectionTask = new TaskCompletionSource<bool>();

                // Handle the Open event to complete the task when the connection is established
                //jobws.OnOpen += (sender, e) =>
                //{
                //    jobWsconnectionTask.SetResult(true);
                //    Log.Information(" job Websocket is open");
                //};
                // Log the initiation of the WebSocket connection
                Log.Information("Service is trying to connect to the job WebSocket!");

                //// Connect to the WebSocket asynchronously
                //await Task.Run(() => jobws.Connect());

                //// Wait for the connection task to complete
                //await jobWsconnectionTask.Task;


                // Log the successful WebSocket connection
                Log.Information("Service connected to the job WebSocket!");
                //await SendMetaData();
            });
        }

        private static void jobWSOnError(object sender, ErrorEventArgs e)
        {
            Log.Error("jobws:" + e.Message);
        }

        private static void jobWSOnCloseAsync(object sender, CloseEventArgs e)
        {
            Log.Error($"jobws is closed: : {e.Reason}");
        }

        private static void jobWebSocketISR(object sender, MessageEventArgs e)
        {
            Data data = JsonConvert.DeserializeObject<Data>(e.Data);
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    Task.Run(async () =>
                    {
                        await webClient.DownloadFileTaskAsync(data.package.path, Globals.watcherSrcPath);
                        Log.Information("File downloaded successfully.");
                    });

                }
                catch (Exception ex)
                {
                    Log.Information("An error occurred while downloading the file: " + ex.Message);
                }
            }

        }
        private static async Task SendMetaData()
        {
            var MetaData = JsonConvert.SerializeObject(new RobotMetaData
            {
                _event = "client robot metaData",
                _machine = new MachineInfo
                {
                    robotName = "LAPTOP-TAUNF8FD",
                    robotAddress = "001AFFDB45C2",
                    userID = 25
                }
            });
            await Task.Run(() =>
            {
                jobws.Send(MetaData);
            });
        }
    }
}
