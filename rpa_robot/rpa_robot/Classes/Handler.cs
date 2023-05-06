using System;
using System.Activities.XamlIntegration;
using System.Activities;
using System.Collections.Generic;
using System.Windows;
using rpa_robot.Classes;
using Serilog;
using System.Threading;
using System.ComponentModel;
using System.IO;


namespace rpa_robot
{
    public class Handler
    {
        public static Queue<string> LogQueue = new Queue<string>();
        private static bool WorkFlowCreated = false;
        private static bool LastWorkFlowDone = true;
        public static void RunWorkFlow()
        {
            try
            {
                var workflow = ActivityXamlServices.Load(Globals.WorkflowFilePath);
                var wa = new WorkflowApplication(workflow);
                wa.Extensions.Add(new AsyncTrackingParticipant());
                wa.Run();
            }
            catch (Exception)
            {
                MessageBox.Show("Error while running the Workflow!");
            }

        }
        public static void LoggingProcess(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (WorkFlowCreated && LastWorkFlowDone) 
                {
                    WorkFlowCreated = false;
                    StartWorkFlowThread();
                }
                if ((LogQueue.Count > 0) || (Orchestrator.OrchestratorProcessQueue.Count > 0))
                {
                    string log = "";
                    lock (LogQueue)
                    {
                        log = LogQueue.Dequeue();

                    }
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

                    Thread.Sleep(500);

                    if (Orchestrator.OrchestratorProcessQueue.Count > 0)
                    {
                        Log.Information(Orchestrator.OrchestratorProcessQueue.Dequeue());
                    }
                }
                else
                {
                    //== THIS LINE IS WRITTEN TO AVOID THE OVEDHEAD DUE TO THE WHILE LOOP, LOOPING ON NOTHING ==//
                    Thread.Sleep(500);
                }
            }

        }

        internal static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Log.Information($"FileCreated: {e.FullPath}");
            CpyWorkFlow();
            DeleteeWorkFlow(Globals.sourceFilePath);
            WorkFlowCreated = true;
        }
        private static void CpyWorkFlow() 
        {
            try
            {
                // Copy the file
                File.Copy(Globals.sourceFilePath, Globals.destinationFilePath,true);
                Log.Information("File copied successfully.");
            }
            catch (IOException er)
            {
                Log.Information($"An error occurred while copying the file: {er.Message}");
            }
            catch (Exception ex)
            {
                Log.Information($"An error occurred: {ex.Message}");
            }
        }

        private static void DeleteeWorkFlow(string Path) 
        {
            try
            {
                // Delete the file
                File.Delete(Path);
                Log.Information("File deleted successfully.");
            }
            catch (IOException er)
            {
                Log.Information($"An error occurred while deleting the file: {er.Message}");
            }
            catch (Exception ex)
            {
                Log.Information($"An error occurred: {ex.Message}");
            }
        }
        private static void StartWorkFlowThread()
        {
            Thread thread = new Thread(ThreadMethod);
            thread.Start();
        }

        private static void ThreadMethod()
        {
            LastWorkFlowDone = false;
            RunWorkFlow();   
            LastWorkFlowDone = true;
        }
    }
}
