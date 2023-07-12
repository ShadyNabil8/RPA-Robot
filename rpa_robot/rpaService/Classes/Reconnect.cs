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
    internal class Reconnect
    {
        public static bool loggingWspingReceived = false;
        public static bool jobWspingReceived = false;
        public static int timeoutDuration = 20000;
        public static Timer timer;
        //public static Timer timer = new Timer(TimerElapsed, null, timeoutDuration, Timeout.Infinite);
        public static void TimerElapsed(object state)
        {
            Log.Information("Timer Elapsed");

            if (!loggingWspingReceived)
            {
                Log.Information("Loggingws: No ping message received for the specified duration.");
                if (CheckInternetConnection())
                {
                    Task.Run(async () =>
                    {
                        if (Handler.CheckForValidUsername() && Handler.CheckForValidPassword())
                        {
                            await Orchestrator.MakeAuthenticationAsync();
                        }
                        else
                        {
                            Log.Information("No Authentication because there is no username or password reconnect");
                        }
                    });
                }
            }
            if (!jobWspingReceived)
            {
                Log.Information("jobws: No ping message received for the specified duration.");
                if (CheckInternetConnection())
                {
                    Task.Run(async () =>
                    {
                        if (Handler.CheckForValidUserID())
                        {
                            await job.JobWsInit();
                        }
                        else
                        {
                            Log.Information("No Authentication because there is no uuid from reconnect");
                        }
                    });
                }
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
    }
}
