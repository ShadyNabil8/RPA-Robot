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
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Windows.Shapes;


namespace rpa_robot
{
    public class Handler
    {
        public static Queue<string> LogQueue = new Queue<string>();
        private static bool WorkFlowCreated = false;
        private static bool LastWorkFlowDone = true;

        /// <summary>
        /// Loads and executes a workflow defined in XAML format.
        /// </summary>
        /// <remarks>
        /// Loads the workflow from the specified XAML file path.
        /// Creates a WorkflowApplication instance with the loaded workflow.
        /// Adds an AsyncTrackingParticipant extension for tracking workflow execution.
        /// Subscribes to the Completed event to handle workflow completion.
        /// Subscribes to the OnUnhandledException event to handle unhandled exceptions.
        /// Runs the workflow.
        /// </remarks>
        public static void RunWorkFlow()
        {
            try
            {
                var workflow = ActivityXamlServices.Load(Globals.WorkflowFilePath);

                var wa = new WorkflowApplication(workflow);

                wa.Extensions.Add(new AsyncTrackingParticipant());

                // Subscribe to the Completed event
                wa.Completed += WorkflowCompleted;

                // Subscribe to the Unhandled Exception event
                wa.OnUnhandledException += WorkflowUnhandledException;


                wa.Run();
            }
            catch (Exception)
            {
                MessageBox.Show("Error while running the Workflow!");
            }

        }
        /*====================================================================================================================*/
        private static UnhandledExceptionAction WorkflowUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs arg)
        {
            // Handle the unhandled exception
            MessageBox.Show($"Unhandled exception occurred: {arg.UnhandledException.Message}");

            // Specify the action to take after handling the exception
            // For example, you can choose to terminate the workflow or abort it
            return UnhandledExceptionAction.Terminate;
        }

        private static void WorkflowCompleted(WorkflowApplicationCompletedEventArgs obj)
        {
            if (obj.CompletionState == ActivityInstanceState.Closed)
            {
                // Workflow execution completed successfully
                LastWorkFlowDone = true;
                Log.Information("Workflow completed successfully!");
                Globals.uiDispatcher.Invoke(() =>
                {
                    Globals.LogsTxtBox.AppendText("Workflow completed successfully!\n");
                });
            }
            else if (obj.CompletionState == ActivityInstanceState.Canceled)
            {
                // Workflow execution was canceled
                Log.Information("Workflow was canceled!");
            }
            else if (obj.CompletionState == ActivityInstanceState.Faulted)
            {
                // Workflow execution encountered an error
                Log.Information("Workflow encountered an error!");
            }
        }
        /*====================================================================================================================*/

        /// <summary>
        /// Handles the logging process by sending log data to an orchestrator and processing orchestrator process queue.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// Functionality:
        /// Checks for conditions to send log data and process the orchestrator process queue in an infinite loop.
        /// If a new workflow is created and the last workflow is done, triggers workflow-related actions (e.g., copying workflow, deleting original file, starting workflow thread).
        /// Sends log data from the LogQueue to the orchestrator using an asynchronous method.
        /// Sleeps for a specified interval before checking the conditions again.
        /// </remarks>
        public static void LoggingProcess(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                // Check if a workflow has been created and the last workflow is done
                if (WorkFlowCreated && LastWorkFlowDone)
                {
                    // Reset the flag for workflow creation and perform necessary actions
                    WorkFlowCreated = false;

                    // Copy the work flow from the source path to the destination path to be ready for another workflow.
                    CpyWorkFlow();

                    // Delete the work flow from the source path to be ready for another workflow.
                    DeleteeWorkFlow(Globals.sourceFilePath);

                    // Create a thread to run the workflow.
                    StartWorkFlowThread();
                }
                // Check if there are logs in the log queue or orchestrator events in the process queue
                if ((LogQueue.Count > 0) || (Orchestrator.OrchestratorProcessQueue.Count > 0))
                {
                    string log = "";
                    lock (LogQueue)
                    {
                        // Dequeue a log from the log queue
                        log = LogQueue.Dequeue();

                    }
                    // Send the log asynchronously to the orchestrator WebSocket
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

                    // Pause the thread for a short period to prevent excessive loading on the server
                    Thread.Sleep(500);

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
                    // Pause the thread for a short period to prevent excessive looping
                    Thread.Sleep(500);
                }
            }

        }
        /*====================================================================================================================*/

        /// <summary>
        /// Event handler for the file creation event.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The event arguments containing information about the created file.</param>
        /// <remarks>
        /// Functionality:
        /// Called when a new file is created in the specified directory.
        /// Logs the full path of the created file.
        /// Optionally performs additional actions related to the workflow (e.g., copying workflow, deleting original file, triggering workflow execution).
        /// Sets a flag indicating that a workflow has been created.
        /// </remarks>
        internal static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Log.Information($"FileCreated: {e.FullPath}");
            //CpyWorkFlow();
            //DeleteeWorkFlow(Globals.sourceFilePath);
            WorkFlowCreated = true;
        }
        /*====================================================================================================================*/

        /// <summary>
        /// Copies the workflow file from the source directory to the destination directory.
        /// </summary>
        /// <remarks>
        /// Functionality:
        /// Copies the workflow file from the source directory to the destination directory.
        /// Logs the outcome of the file copying process, including any errors encountered.
        /// </remarks>
        private static void CpyWorkFlow()
        {
            try
            {
                // Copy the file
                File.Copy(Globals.sourceFilePath, Globals.destinationFilePath, true);
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
        /*====================================================================================================================*/

        /// <summary>
        /// Deletes the workflow file at the specified path.
        /// </summary>
        /// <param name="Path">The path of the file to be deleted.</param>
        /// <remarks>
        /// Deletes the workflow file at the specified path.
        /// Logs the outcome of the file deletion process, including any errors encountered.
        /// </remarks>
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
        /*====================================================================================================================*/

        /// <summary>
        /// Starts a new thread to execute the workflow.
        /// </summary>
        /// <remarks>
        /// Creates a new thread and starts its execution by invoking the ThreadMethod.
        /// Typically used to execute the workflow asynchronously in a separate thread.
        /// </remarks>
        private static void StartWorkFlowThread()
        {
            Thread thread = new Thread(ThreadMethod);
            thread.Start();
        }
        /*====================================================================================================================*/

        /// <summary>
        /// Method executed by the separate thread to run the workflow.
        /// </summary>
        /// <remarks>
        /// Sets the flag indicating that the last workflow execution is not yet done.
        /// Invokes the RunWorkFlow function to start the execution of the workflow.
        /// Optionally sets the flag indicating that the last workflow execution is done.
        /// It is typically used in conjunction with StartWorkFlowThread to execute the workflow asynchronously.
        /// </remarks>
        private static void ThreadMethod()
        {
            LastWorkFlowDone = false;
            RunWorkFlow();
        }
    }
}
