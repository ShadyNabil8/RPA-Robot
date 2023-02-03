using System.Activities;
using System.Activities.XamlIntegration;
using System.IO;
using System.Text;

namespace WorkFlowRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"E:\CSE\4th comp\grad.Proj\XAMLs\Workflow3.xaml";
            string tempString = "";
            StringBuilder xamlWFString = new StringBuilder();
            StreamReader xamlStreamReader =
                new StreamReader(filePath);
            while (tempString != null)
            {
                tempString = xamlStreamReader.ReadLine();
                if (tempString != null)
                {
                    xamlWFString.Append(tempString);
                }
            }
            Activity wfInstance = ActivityXamlServices.Load(
                new StringReader(xamlWFString.ToString()));
            WorkflowInvoker.Invoke(wfInstance);
            //try
            //{
            //    WorkflowInvoker.Invoke(wfInstance, TimeSpan.FromSeconds(10));
            //}

            //catch (TimeoutException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
