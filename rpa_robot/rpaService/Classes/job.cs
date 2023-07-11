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
using System.Net.NetworkInformation;

namespace rpaService.Classes
{
    internal class job
    {
        public static WebSocket jobws = null; /* workflow ws */
        //private static int MaxRetryCount = 10;
        public static async Task JobWsInit()
        {
            try
            {
                await ConnectToJobWs();
            }
            catch (Exception)
            {

                Log.Information("Error in connecting to job ws");
            }

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
            if (e.IsPing)
            {
                Log.Information("jobws => Ping");
                Reconnect.jobWspingReceived = true;
            }
            else
            {
                Data data = null;
                bool DeserializeObjectDone = false;
                Log.Information(e.Data.ToString());
                try
                {
                    data = JsonConvert.DeserializeObject<Data>(e.Data.ToString());
                    DeserializeObjectDone = true;
                }
                catch (Exception)
                {

                    Log.Information("jobws => Error in Deserializing the Object");
                }
                if (DeserializeObjectDone)
                {
                    if (!data._event.Equals("Decline metaData reception"))
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            Package pkg = null;
                            try
                            {
                                pkg = JsonConvert.DeserializeObject<Package>(data.value);
                            }
                            catch (Exception)
                            {

                                Log.Information("jobws => Error in Deserializing the package");
                            }
                            try
                            {
                                Task.Run( () =>
                                {
                                    webClient.DownloadFile(pkg.path, Globals.downloadPath);
                                    Log.Information("File downloaded successfully.");
                                });

                            }
                            catch (Exception ex)
                            {
                                Log.Information("jobws => An error occurred while downloading the file: " + ex.Message);
                            }
                        }

                    }
                    else
                    {
                        Log.Information(data._event);
                        Task.Run(async () =>
                        {
                            await SendMetaData();
                        });
                    }
                }
                else
                {
                    Log.Information("jobws => Deserializing failled");
                    var MetaData = JsonConvert.SerializeObject(new Data
                    {
                        _event = "Decline pkg reception",
                        value = ""
                    });
                    jobws.SendAsync(MetaData, (completed) =>
                    {
                        if (completed)
                        {
                            Log.Information("Decline pkg reception sent successfully!");
                        }
                        else
                        {
                            Log.Information("Failed to send Decline pkg reception.");
                        }
                    });
                }

            }
        }
        private static async Task SendMetaData()
        {
            string macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            
            var MetaData = JsonConvert.SerializeObject(new RobotMetaData
            {
                _event = "client robot metaData",
                value = new MachineInfo
                {
                    //robotName = Environment.MachineName,
                    robotName = Environment.MachineName,
                    robotAddress = macAddr,
                    uuid = Globals.uuid /* NOOOOOOOOOOOTE HERE*/
                }
            });
            Log.Information(MetaData);

            try
            {
                await Task.Run(() =>
                {
                    jobws.Send(MetaData);
                    Log.Information("jobws => Meta data sent");
                });
            }
            catch (Exception ex)
            {

                Log.Information("jobws => Failed to send robot meta date" + ex.Message);
            }

        }
        static async Task ConnectToJobWs()
        {
            jobws = new WebSocket(Globals.jobEndPoint);

            // Subscribe to WebSocket events
            jobws.OnMessage += jobWebSocketISR;
            jobws.OnClose += jobWSOnCloseAsync;
            jobws.OnError += jobWSOnError;
            jobws.EmitOnPing = true;

            // Create a task completion source to track the completion of the connection
            var jobWsconnectionTask = new TaskCompletionSource<bool>();

            // Handle the Open event to complete the task when the connection is established
            jobws.OnOpen += (sender, e) =>
            {
                jobWsconnectionTask.SetResult(true);
                Log.Information("job Websocket is open");
            };
            // Log the initiation of the WebSocket connection
            Log.Information("Service is trying to connect to the job WebSocket!");

            //// Connect to the WebSocket asynchronously
            await Task.Run(() => jobws.Connect());

            //// Wait for the connection task to complete
            await jobWsconnectionTask.Task;


            // Log the successful WebSocket connection
            Log.Information("Service connected to the job WebSocket!");
            await SendMetaData();

        }
    }
}
