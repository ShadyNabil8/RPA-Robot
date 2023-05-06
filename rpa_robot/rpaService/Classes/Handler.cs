using Newtonsoft.Json;
using rpaService.Formats;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

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
                if ((Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Count > 0) || (Orchestrator.OrchestratorProcessQueue.Count > 0))
                { 
                    if (Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Count > 0)
                    {
                        string log = "";
                        lock (Globals.ServiceAsyncListenerFromRobot.ProcessQueue)
                        {
                            log = Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Dequeue();
                            //Log.Information(Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Dequeue());
                            //Log.Information(Globals.ServiceAsyncListenerFromRobot.ProcessQueue.Dequeue() + "form Q");
                        }
                        Log.Information(log);
                        //Orchestrator.ws.Send(log);
                        Orchestrator.ws.SendAsync(log, (completed) =>
                        {
                            if (completed)
                            {
                                Log.Information("Data sent successfully!");
                            }
                            else
                            {
                                Log.Information("Failed to send data.");
                            }
                        });
                        //Log.Information(Orchestrator.Transaction.ToString());
                        //while (Orchestrator.Transaction) ;
                        //Log.Information(Orchestrator.Transaction.ToString());
                        //Orchestrator.Transaction = true;
                        //Globals.ServiceAsyncClientFromRobot.StartClient("HISHADY");
                    }
                    if (Orchestrator.OrchestratorProcessQueue.Count > 0)
                    {
                        lock (Globals.ServiceAsyncListenerFromRobot.ProcessQueue)
                        {
                            Log.Information(Orchestrator.OrchestratorProcessQueue.Dequeue());
                            //Orchestrator.ws.Send(Orchestrator.OrchestratorProcessQueue.Dequeue());
                        }
                    }
                }
                else
                {
                    /*if(Orchestrator.Connected)
                    {
                        var log = JsonConvert.SerializeObject(new RpaLog
                        {
                            eventType = "logEmitEvent",
                            payload = new Payload { logType = "ERROR", name = "MessageBox", status = "Running", timestamp = "12345", message = "this is a log entry", robotId = 1 }
                        });
                        Orchestrator.ws.Send(log);
                    }*/
                    
                    //== THIS LINE IS WRITTEN TO AVOID THE OVEDHEAD DUE TO THE WHILE LOOP, LOOPING ON NOTHING ==//
                    Thread.Sleep(100);
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
