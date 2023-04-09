using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpaService.Classes
{
    internal class Handler
    {
        /*
         * MainLoop()
         * ServiceAsyncListenerFromRobotFun()
         * ServiceAsyncListenerFromRobotThreadHnadlerFun()
         */

        public static void MainLoop() 
        {
            while (true)
            {
                if (Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Count > 0)
                {
                    lock (Globals.ServiceAsyncListenerFromRobot.ProcessQueue)
                    {
                        Log.Information(Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Dequeue() + "form Q");
                    }
                    Globals.ServiceAsyncClientFromRobot.StartClient("HISHADY");
                }
                else
                {
                    //== THIS LINE IS WRITTEN TO AVOID THE OVEDHEAD DUE TO THE WHILE LOOP, LOOPING ON NOTHING ==//
                    Thread.Sleep(1000);
                }
            }
        }
        public static void ServiceAsyncListenerFromRobotFun()
        {
            Globals.ServiceAsyncListenerFromRobot.StartListening();
        }

        public static void ServiceAsyncListenerFromRobotThreadHnadlerFun()
        {
            MainLoop();
        }
    }
}
