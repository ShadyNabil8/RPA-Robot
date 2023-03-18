using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace rpa_robot.Classes
{
    public class Service
    {
        private static string serviceName = "rpaService";
        private static string serviceExePath = @"D:\New folder\CSE\grad.Proj\rpaSlayerRobot\rpaService\bin\Debug\rpaService.exe";
        private static TransactedInstaller transactedInstaller;
        private static ServiceProcessInstaller serviceProcessInstaller;
        private static bool isInstalled = false;
        public static void Initialize()
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == serviceName)
                {
                    isInstalled = true;
                    break;
                }
            }
            if (!isInstalled)
            {
                transactedInstaller = new TransactedInstaller();

                // Create a new instance of ServiceProcessInstaller
                serviceProcessInstaller = new ServiceProcessInstaller();
                serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

                // Create a new instance of ServiceInstaller
                ServiceInstaller serviceInstaller = new ServiceInstaller();
                serviceInstaller.StartType = ServiceStartMode.Manual;
                serviceInstaller.ServiceName = serviceName;
                serviceInstaller.DisplayName = serviceName;
                serviceInstaller.Description = "My Windows Service";
                serviceInstaller.DelayedAutoStart = true;

                // Set the service executable path
                serviceInstaller.Context = new InstallContext();
                serviceInstaller.Context.Parameters["assemblypath"] = serviceExePath;
                serviceInstaller.BeforeUninstall += new InstallEventHandler(MyInstaller_BeforeUninstall);

                // Install the service
                transactedInstaller = new TransactedInstaller();
                transactedInstaller.Installers.Add(serviceProcessInstaller);
                transactedInstaller.Installers.Add(serviceInstaller);
                transactedInstaller.Context = new InstallContext();
                transactedInstaller.Context.Parameters["assemblypath"] = serviceExePath;
            }
            else 
            {
                Log.Information(Info.SERVICE_INSTALLED);
                isInstalled = true;
            }
            
        }
        private static void MyInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.Status != ServiceControllerStatus.Stopped)
            {
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
        }
        public static void InstallAndStart() 
        {
            if (!isInstalled)
            {
                transactedInstaller.Install(new System.Collections.Hashtable());
                Log.Information(Info.SERVICE_INSTALLED);
                Start();
                isInstalled = true;
            }
            
        }

        public static void UninstallAndStop() 
        {
            if (isInstalled)
            {
                transactedInstaller.Uninstall(null);
                Log.Information(Info.SERVICE_UNINSTALLED);
                isInstalled = false;
            }
            else 
            {
                Log.Information(Info.SERVICE_IS_ALREADY_UNINSTALLED);
            }
        }
        public static void Start()
        {
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                serviceController.Start();
                Log.Information(Info.SERVICE_STARTED);
            }
            else 
            {
                Log.Information(Info.SERVICE_IS_ALREADY_STARTED);
            }
            
            
        }

        public static void Stop()
        {
            ServiceController serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                serviceController.Stop();
                Log.Information(Info.SERVICE_STOPED);
            }
            else
            {
                Log.Information(Info.SERVICE_IS_ALREADY_STOPED);
            }
        }

    }
}
