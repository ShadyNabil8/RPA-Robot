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
using System.ServiceProcess;
using System.Reflection.Emit;

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
            var workflow = ActivityXamlServices.Load(Globals.WorkflowFilePath);

            var wa = new WorkflowApplication(workflow);

            wa.Extensions.Add(new AsyncTrackingParticipant());

            // Subscribe to the Completed event
            wa.Completed += WorkflowCompleted;

            // Subscribe to the Unhandled Exception event
            wa.OnUnhandledException += WorkflowUnhandledException;

            wa.Run();
        }

        /// <summary>
        /// Event handler for unhandled exceptions in the workflow application.
        /// </summary>
        /// <param name="arg">The WorkflowApplicationUnhandledExceptionEventArgs containing the unhandled exception information.</param>
        /// <returns>The action to take after handling the exception.</returns>
        private static UnhandledExceptionAction WorkflowUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs arg)
        {
            // Handle the unhandled exception by displaying a message box with the exception message
            MessageBox.Show($"Unhandled exception occurred: {arg.UnhandledException.Message}");

            // Specify the action to take after handling the exception
            // For example, you can choose to terminate the workflow or abort it
            return UnhandledExceptionAction.Terminate;
        }


        /// <summary>
        /// Event handler for workflow completion.
        /// </summary>
        /// <param name="obj">The WorkflowApplicationCompletedEventArgs containing the completion event information.</param>
        private static void WorkflowCompleted(WorkflowApplicationCompletedEventArgs obj)
        {
            if (obj.CompletionState == ActivityInstanceState.Closed)
            {
                // Workflow execution completed successfully
                //LastWorkFlowDone = true;
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

        /// <summary>
        /// The code above defines a background worker method called RobotProcess that runs in a separate thread and continuously performs the robot process. Here's a detailed breakdown of the function:
        /// The method starts an infinite loop using while (true) to keep the process running continuously.
        /// It checks if both the WorkFlowCreated and LastWorkFlowDone flags are true using the condition WorkFlowCreated && LastWorkFlowDone.
        /// If the conditions are met, it enters the if block and performs the necessary actions for workflow creation.
        /// Within the if block, the WorkFlowCreated flag is reset to false.
        /// It calls the CpyWorkFlow() method to copy the workflow from the source path to the destination path, preparing for another workflow.
        /// It calls the DeleteeWorkFlow() method to delete the workflow from the source path, ensuring it's ready for another workflow.
        /// It creates a thread to run the workflow by calling the StartWorkFlowThread() method.
        /// If there are logs in the LogQueue, it enters the inner if block.
        /// Within the inner if block, the LogQueue is locked using lock (LogQueue) to ensure thread safety during dequeueing.
        /// It dequeues a log from the LogQueue using LogQueue.Dequeue() and sends it to the socket using the AsynchronousClient.SendToSocket() method.
        /// After sending the log, the thread sleeps for a short duration of 100 milliseconds using Thread.Sleep(100) to prevent excessive processing and to give other threads a chance to execute.
        /// If there are no logs in the queue, it executes the else block.
        /// The purpose of the else block is to avoid excessive looping when there are no logs to process. It simply sleeps for a short duration of 100 milliseconds using Thread.Sleep(100).
        /// The loop continues, repeating the process of checking and processing logs and performing necessary workflow actions as long as the program is running.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The DoWorkEventArgs containing event data.</param>
        public static void RobotProcess(object sender, DoWorkEventArgs e)
        {
            //bool seviceInstalled = IsServiceInstalled();

            while (true)
            {
                if (WorkFlowCreated /*&& LastWorkFlowDone*/)
                {
                    // Reset the flag for workflow creation and perform necessary actions
                    WorkFlowCreated = false;
                    //DeleteeWorkFlow(Globals.sourceFilePath);
                    // Copy the work flow from the source path to the destination path to be ready for another workflow.
                    CpyWorkFlow();

                    // Delete the work flow from the source path to be ready for another workflow.
                    DeleteeWorkFlow(Globals.sourceFilePath);

                    // Create a thread to run the workflow.
                    StartWorkFlowThread();
                }
                //if (File.Exists(Globals.sourceFilePath))
                //{
                //    File.Delete(Globals.sourceFilePath);
                //}
                if (LogQueue.Count > 0)
                {
                    lock (LogQueue)
                    {
                        // Dequeue a log from the log queue
                        // Send the log to the socket using the AsynchronousClient
                        AsynchronousClient.StartClient(LogQueue.Dequeue());
                    }
                }
                // Sleep for a short period to avoid excessive looping when there are no logs to process
                Thread.Sleep(100);
            }

        }

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

            // Wait for the file to be completely downloaded
            bool isFileDownloaded = false;
            int timeoutSeconds = 30;
            int elapsedSeconds = 0;
            string filePath = e.FullPath;

            while (!isFileDownloaded && elapsedSeconds < timeoutSeconds)
            {
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    if (fileInfo.Length > 0)
                    {
                        isFileDownloaded = true;
                    }
                }

                Thread.Sleep(1000);
                elapsedSeconds++;
            }
            if (isFileDownloaded)
            {
                // File is completely downloaded
                Log.Information($"File downloaded: {e.FullPath}");
                // Proceed with further operations or workflows
                Log.Information($"FileCreated: {e.FullPath}");
                DeleteeWorkFlow(Globals.destinationFilePath);
                //CpyWorkFlow();
                //Thread.Sleep(10);
                //DeleteeWorkFlow(Globals.sourceFilePath);
                WorkFlowCreated = true;
            }
            else
            {
                // File download timed out or encountered an error
                Log.Warning($"File download failed: {e.FullPath}");
                // Handle the failure or cleanup operations
            }

        }


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
                if (File.Exists(Globals.sourceFilePath))
                {
                    File.Copy(Globals.sourceFilePath, Globals.destinationFilePath, true);
                    Log.Information("File copied successfully.");
                }
                //File.Copy(Globals.sourceFilePath, Globals.destinationFilePath, true);

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
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    Log.Information("File deleted successfully.");
                }
                //File.Delete(Path);    
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
            //LastWorkFlowDone = false;
            try
            {
                RunWorkFlow();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        public static bool IsServiceInstalled()
        {
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName.Equals(Globals.serviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static string ReadUserID() 
        {
            string uuid = null;
            if (File.Exists(Globals.userIdFile))
            {
                FileInfo fileInfo = new FileInfo(Globals.userIdFile);
                if (fileInfo.Length > 0)
                {
                    try
                    {
                        uuid = File.ReadAllText(Globals.userIdFile);
                        Log.Information($"uuid: {uuid}");
                    }
                    catch (Exception ex)
                    {

                        Log.Information($"Error while reading the userid {ex.Message}");
                    }
                }
                else
                {
                    Log.Information("userid is empty");
                }
            }
            else
            {
                Log.Information("userid does not exist");
            }
            return uuid;
        }

        public static string ReadRobotAd() 
        {
            string ra = null;
            if (File.Exists(Globals.robotAddressFile))
            {
                FileInfo fileInfo = new FileInfo(Globals.robotAddressFile);
                if (fileInfo.Length > 0)
                {
                    try
                    {
                        ra = File.ReadAllText(Globals.robotAddressFile);
                        Log.Information($"robotAddress: {ra}");
                    }
                    catch (Exception ex)
                    {

                        Log.Information($"Error while reading the robot address: {ex.Message}");
                    }
                }
                else
                {
                    Log.Information("robot address is empty");
                }
            }
            else
            {
                Log.Information("robot address does not exist");
            }
            return ra;       
        }
    }
}
