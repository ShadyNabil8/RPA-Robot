using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;


namespace rpaService.Classes
{
    internal class Reconnector
    {
        public static bool loggingWspingReceived = false;
        public static bool jobWspingReceived = false;
        public static int timeoutDuration = 20000;
        public static Timer timer;
        public static async void TimerElapsed(object state)
        {
            Log.Information("Timer Elapsed");
            if (!loggingWspingReceived)
            {
                Log.Warning("Loggingws: No ping message received for the specified duration.");
                await LogWebSocketConnect();
            }
            if (!jobWspingReceived)
            {
                Log.Warning("jobws: No ping message received for the specified duration.");
                await PackageReceiverSocketConnect();
            }
            loggingWspingReceived = false;
            jobWspingReceived = false;

            // Restart the timer
            timer.Change(timeoutDuration, Timeout.Infinite);
        }
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 2000); // Use a reliable IP address for checking internet connectivity, like Google's DNS server (8.8.8.8)
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        public static async Task LogWebSocketConnect()
        {
            if (CheckInternetConnection())
            {
                await Task.Run(async () => { await LogSender.MakeAuthenticationAsync(); });
            }

        }
        public static async Task PackageReceiverSocketConnect()
        {

            if (CheckInternetConnection())
            {
                await Task.Run(async () => { await PackageReceiver.ConnectAsync(); });
            }
        }
        public static async void Reconnect()
        {
            loggingWspingReceived = true;
            jobWspingReceived = true;
            await LogWebSocketConnect();
            await PackageReceiverSocketConnect();
        }

    }
}
