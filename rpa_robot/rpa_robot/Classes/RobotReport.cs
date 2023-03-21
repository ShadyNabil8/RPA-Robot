using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpa_robot.Classes
{
    internal class RobotReport
    {
        public string Report { get; set; }
        public string Type { get; set; }
        public RobotReport(string Report, string Type) 
        {
            this.Report = Report;
            this.Type = Type;
        }
    }
}
