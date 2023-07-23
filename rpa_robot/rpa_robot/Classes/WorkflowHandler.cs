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
    public class WorkflowHandler
    {
        
        public static bool WorkFlowCreated = false;
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
        private static UnhandledExceptionAction WorkflowUnhandledException(WorkflowApplicationUnhandledExceptionEventArgs arg)
        {
            // Handle the unhandled exception by displaying a message box with the exception message
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
                //LastWorkFlowDone = true;
                Log.Information("Workflow completed successfully!");
                Helper.PrintOnUI("Workflow completed successfully!");
            }
            else if (obj.CompletionState == ActivityInstanceState.Canceled)
            {
                // Workflow execution was canceled
                Log.Information("Workflow was canceled!");
                Helper.PrintOnUI("Workflow was canceled!");
            }
            else if (obj.CompletionState == ActivityInstanceState.Faulted)
            {
                // Workflow execution encountered an error
                Log.Information("Workflow encountered an error!");
                Helper.PrintOnUI("Workflow encountered an error!");
            }
        }
  
        private static void StartWorkFlowThread()
        {
            Thread thread = new Thread(ThreadMethod);
            thread.Start();
        }

        private static void ThreadMethod()
        {
            //LastWorkFlowDone = false;
            try
            {
                RunWorkFlow();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error while running wf {ex.Message}");
            }

        }

        public static void OnFileCreated(object sender, EventArgs e)
        {
            Helper.CpyWorkFlow();
            Helper.DeleteeWorkFlow(Globals.sourceFilePath);
            StartWorkFlowThread();
        }

    }
}
