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
    internal class PackageReceiver
    {
        public static WebSocket PackageReceiverSocket = null;
        public static async Task ConnectAsync()
        {
            try
            {
                if (Helper.CheckForValidUuid())
                {
                    await ConnectToJobWs();
                }
                else
                {
                    Log.Error("Can not connecto to PackageReceiverSocket(invalid uuid)");
                }
            }
            catch (Exception)
            {

                Log.Error("Error in connecting to job ws");
            }

        }
        private static void PackageReceiverSocketOnError(object sender, ErrorEventArgs e)
        {
            Log.Error("PackageReceiverSocket:" + e.Message);
        }

        private static void PackageReceiverSocketOnCloseAsync(object sender, CloseEventArgs e)
        {
            Log.Error($"PackageReceiverSocket is closed: : {e.Reason}");
        }

        private static void PackageReceiverSocketISR(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                Log.Information("Ping!!");
                Reconnector.jobWspingReceived = true;
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

                    Log.Information("PackageReceiverSocket: Error in Deserializing the msg from the orchestrator");
                }
                if (DeserializeObjectDone)
                {
                    if (!data._event.Equals("Decline metaData reception"))
                    {
                        Package pkg = null;
                        try
                        {
                            pkg = JsonConvert.DeserializeObject<Package>(data.value);
                        }
                        catch (Exception)
                        {
                            Log.Error("PackageReceiverSocket: Error in Deserializing the package");
                            DeserializeObjectDone = false;
                        }
                        if (DeserializeObjectDone)
                        {
                            Helper.DownloadPackage(pkg.path);
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
                    Log.Error("PackageReceiverSocket: Package deserializing failled");
                    var MetaData = JsonConvert.SerializeObject(new Data
                    {
                        _event = "Decline pkg reception",
                        value = ""
                    });
                    PackageReceiverSocket.SendAsync(MetaData, (completed) =>
                    {
                        if (completed)
                        {
                            Log.Information("Decline pkg reception sent successfully!");
                        }
                        else
                        {
                            Log.Error("Failed to send Decline pkg reception.");
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
                    robotName = Environment.MachineName,
                    robotAddress = macAddr,
                    uuid = Globals.uuid
                }
            });
            Log.Information(MetaData);

            try
            {
                await Task.Run(() =>
                {
                    PackageReceiverSocket.Send(MetaData);
                    Log.Error("PackageReceiverSocket: Meta data sent");
                });
            }
            catch (Exception ex)
            {

                Log.Error("PackageReceiverSocket: Failed to send robot meta date" + ex.Message);
            }

        }
        static async Task ConnectToJobWs()
        {
            Log.Information("Trying to connect to PackageReceiverSockett!");
            PackageReceiverSocket = new WebSocket(Globals.jobEndPoint);
            PackageReceiverSocket.OnMessage += PackageReceiverSocketISR;
            PackageReceiverSocket.OnClose += PackageReceiverSocketOnCloseAsync;
            PackageReceiverSocket.OnError += PackageReceiverSocketOnError;
            PackageReceiverSocket.EmitOnPing = true;
            var jobWsconnectionTask = new TaskCompletionSource<bool>();
            PackageReceiverSocket.OnOpen += (sender, e) =>
            {
                jobWsconnectionTask.SetResult(true);
                Log.Information("Connected to PackageReceiverSocket!");
            };
            await Task.Run(() => PackageReceiverSocket.Connect());
            await jobWsconnectionTask.Task;
            await SendMetaData();

        }
    }
}
