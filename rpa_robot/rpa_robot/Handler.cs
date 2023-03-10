using System;
using System.Activities.XamlIntegration;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace rpa_robot
{
    public class Handler
    {
        const String WorkflowFilePath = "D:\\New folder\\CSE\\grad.Proj\\XAMLs\\ShowMB.xaml";
        public static void RunWorkFlow()
        {
            if (!string.IsNullOrEmpty(WorkflowFilePath))
            {
                try
                {
                    var workflow = ActivityXamlServices.Load(WorkflowFilePath);
                    var wa = new WorkflowApplication(workflow);
                    wa.Extensions.Add(new AsyncTrackingParticipant());
                    wa.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


        }
    }
}
