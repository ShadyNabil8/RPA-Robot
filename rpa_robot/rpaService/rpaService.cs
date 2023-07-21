using System.ServiceProcess;
using Serilog;
using rpaService.Classes;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using WebSocketSharp;

namespace rpaService
{
    public partial class rpaService : ServiceBase
    {
        
        public rpaService()
        {
            InitializeComponent();
              
        }
        protected override void OnStart(string[] args)
        {
            Initialization.ServiceInit();
            Log.Information("Service started!");
            
        }
        protected override void OnStop()
        {
            Log.Information("Service stopped!");
        }
        protected override void OnContinue()
        {
        }
        protected override void OnShutdown()
        {
            Log.Information("Service shutdown!");
        }

    }
}
