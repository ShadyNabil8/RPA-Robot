using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace rpa_robot.Classes
{
    public class Service
    {
        private static bool isInstalled      = false;
        private static string serviceName    = "rpaService";
        private static string serviceExePath = @"D:\New folder\CSE\grad.Proj\rpa_robot\rpaService\bin\Debug\rpaService.exe";
        private static TransactedInstaller transactedInstaller;
        private static ServiceProcessInstaller serviceProcessInstaller;
        private static ServiceInstaller serviceInstaller;

        public static void Initialize()
        {
            // ==== TO CHECK IF THE SERVICE IS ALREADY INSTALLED OR NOT ====
            ServiceController[] services = ServiceController.GetServices(); //
            foreach (ServiceController service in services)                 //
            {                                                               //
                if (service.ServiceName == serviceName)                     //
                {                                                           //
                    isInstalled = true;                                     //
                    break;                                                  //
                }                                                           //
            }                                                               //
            // =============================================================

            // Create a new instance of ServiceProcessInstaller
            serviceProcessInstaller = new ServiceProcessInstaller();
            // RUN THE SERVICE IN THE LOCAL SYSTEM
            serviceProcessInstaller.Account = ServiceAccount.NetworkService;

            // Create a new instance of ServiceInstaller
            serviceInstaller = new ServiceInstaller();
            // StartType CAN BE AUTOMATIC
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = serviceName;
            serviceInstaller.DisplayName = serviceName;
            serviceInstaller.Description = "RPA_Service";
            serviceInstaller.DelayedAutoStart = false;

            // Set the service executable path
            serviceInstaller.Context = new InstallContext();
            serviceInstaller.Context.Parameters["assemblypath"] = serviceExePath;
            // DEFINE WHAT SHOULD BE HAPPEN BEFORE UNINSTALL THE SERVICE
            serviceInstaller.BeforeUninstall += new InstallEventHandler(MyInstaller_BeforeUninstall);

            // Install the service
            transactedInstaller = new TransactedInstaller();
            transactedInstaller.Installers.Add(serviceProcessInstaller);
            transactedInstaller.Installers.Add(serviceInstaller);
            transactedInstaller.Context = new InstallContext();
            // PASS THE PATH OF THE .EXE SERVICE AS A PARAMETER FOR THE CMD COMMAND
            transactedInstaller.Context.Parameters["assemblypath"] = serviceExePath;

            if (isInstalled == true) 
            {
                // START THE SERVICE ONCE THE ROBOT STARTS
                // SERVICE IS SUPPOSED TO BE INSTALLES SO THAT IT RUNS AUTOMATIC
                // THERE US AN ISSUI; WHEN THE ROBOT STARTS WHITH THE SERVICE INATALLED BUT IS STOPPED
                // THE SERVICE STARTS AGAIN BUT THEE IS NO LOG FOR THAT
                Globals.LogsTxtBox.AppendText(Info.SERVICE_IS_ALREADY_INSTALLED+"\n");
                Start();
            }
            else 
            {
                Globals.LogsTxtBox.AppendText(Info.SERVICE_NEED_TO_BE_INSTALLED +"\n");
            }
        }
        // =============================== STOP THE SERVICE BEFORE UNINSTALL ===============================
        private static void MyInstaller_BeforeUninstall(object sender, InstallEventArgs e)                  //
        {                                                                                                   //  
            ServiceController serviceController = new ServiceController(serviceName);                       //
            if (serviceController.Status != ServiceControllerStatus.Stopped)                                //
            {                                                                                               //
                serviceController.Stop();                                                                   //  
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30)); //  
            }                                                                                               //
        }                                                                                                   //
        // =============================== STOP THE SERVICE BEFORE UNINSTALL ===============================
        public static void InstallAndStart()
        {
            if (!isInstalled)
            {
                transactedInstaller.Install(new System.Collections.Hashtable());
                Thread.Sleep(500);
                Log.Information(Info.SERVICE_INSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_INSTALLED + "\n");
                isInstalled = true;
                Start();
            }
            else
            {
                Log.Information(Info.SERVICE_IS_ALREADY_INSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_IS_ALREADY_INSTALLED + "\n");
            }

        }

        public static void UninstallAndStop()
        {
            if (isInstalled)
            {
                transactedInstaller.Uninstall(null);
                Log.Information(Info.SERVICE_UNINSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_UNINSTALLED + "\n");
                isInstalled = false;
            }
            else
            {
                Log.Information(Info.SERVICE_IS_ALREADY_UNINSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_IS_ALREADY_UNINSTALLED + "\n");
            }
        }
        public static void Start()
        {
            if (isInstalled)
            {
                ServiceController serviceController = new ServiceController(serviceName);
                if (serviceController.Status == ServiceControllerStatus.Stopped)
                {
                    serviceController.Start();
                    Thread.Sleep(500);
                    Log.Information(Info.SERVICE_STARTED);
                    Globals.LogsTxtBox.AppendText(Info.SERVICE_STARTED + "\n");
                }
                else
                {
                    Log.Information(Info.SERVICE_IS_ALREADY_STARTED);
                    Globals.LogsTxtBox.AppendText(Info.SERVICE_IS_ALREADY_STARTED + "\n");
                }
            }
            else 
            {
                Log.Information(Info.SERVICE_NEED_TO_BE_INSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_NEED_TO_BE_INSTALLED + "\n");
            }
        }

        public static void Stop()
        {
            if (isInstalled)
            {
                ServiceController serviceController = new ServiceController(serviceName);
                if (serviceController.Status == ServiceControllerStatus.Running)
                {
                    serviceController.Stop();
                    Thread.Sleep(500);
                    Log.Information(Info.SERVICE_STOPED);
                    Globals.LogsTxtBox.AppendText(Info.SERVICE_STOPED + "\n");


                }
                else
                {
                    Log.Information(Info.SERVICE_IS_ALREADY_STOPED);
                    Globals.LogsTxtBox.AppendText(Info.SERVICE_IS_ALREADY_STOPED + "\n");


                }
            }
            else 
            {
                Log.Information(Info.SERVICE_NEED_TO_BE_INSTALLED);
                Globals.LogsTxtBox.AppendText(Info.SERVICE_NEED_TO_BE_INSTALLED + "\n");
            }   
        }
        public static bool IsInstalled() 
        {
            if (isInstalled)
            {
                return true;
            }
            else 
            {
                return false;
            }
            
        }
        public static bool IsStarted()
        {
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
}
