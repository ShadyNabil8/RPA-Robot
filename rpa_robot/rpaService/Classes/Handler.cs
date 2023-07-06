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

        public static Queue<string> LogQueue = new Queue<string>();
        public static int waitTime = 500;
        public static int ExpBakeOffDelay = 1000;
        public static int Factor = 1;


        /// <summary>
        /// The `LoggingProcessHandler` function handles the logging process by sending logs and orchestrator events to a WebSocket and processing the log queue and orchestrator process queue.
        /// </summary>
        public static void LoggingProcessHandler()
        {
            Task.Run(async () =>
            {
                await Orchestrator.MakeAuthenticationAsync();
                await job.JobWsInit();
            });

            while (true)
            {
                // Check if there are logs in the log queue or orchestrator events in the process queue
                if ((LogQueue.Count > 0) || (Orchestrator.OrchestratorProcessQueue.Count > 0))
                {
                    // Check if there are logs in the log queue
                    if (LogQueue.Count > 0)
                    {
                        string log;
                        lock (LogQueue)
                        {
                            // Dequeue a log from the log queue
                            log = LogQueue.Dequeue();
                        }

                        try
                        {
                            // Check if the WebSocket connection is alive
                            if (Orchestrator.ws.IsAlive)
                            {
                                //Send the log asynchronously using the WebSocket's SendAsync method and handle the completion callback
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

                            }
                        }
                        catch (Exception ex)
                        {
                            // Log the exception that occurred during the sending process
                            Log.Error($"An error occurred during the SendAsync : {ex}");
                        }
                    }
                    // Pause the thread for a short period to prevent excessive loading on the server
                    Thread.Sleep(waitTime);

                    // Check if there are orchestrator events in the process queue
                    if (Orchestrator.OrchestratorProcessQueue.Count > 0)
                    {
                        // Dequeue an orchestrator event from the process queue and log it
                        Log.Information(Orchestrator.OrchestratorProcessQueue.Dequeue());
                    }
                }
                else
                {
                    // No logs or orchestrator events to process
                    // Adjust the factor for exponential backoff
                    if (Factor < 8)
                    {
                        Factor *= 2;
                    }
                    else
                    {
                        Factor = 1;
                    }
                    // No logs or orchestrator events to process
                    // Pause the thread for a short period to prevent excessive looping
                    Thread.Sleep(ExpBakeOffDelay);
                }
            }
        }



        /// <summary>
        /// The ListenerFromRobothHandler function serves as an entry point for starting the asynchronous socket listener.
        /// It simply calls the StartListening method from the AsynchronousSocketListener class to initiate the listening process and handle communication with the robot.
        /// The details of the socket listening and communication logic are encapsulated within the AsynchronousSocketListener class, allowing for modularity and separation of concerns.
        /// </summary>
        public static void ListenerFromRobothHandler()
        {
            // Start the asynchronous socket listener
            AsynchronousSocketListener.StartListening();
        }
    }

}

