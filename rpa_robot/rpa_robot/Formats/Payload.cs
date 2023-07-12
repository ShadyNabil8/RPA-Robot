using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpa_robot.Formats
{
    internal class Payload
    {
        public string logType { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public string message { get; set; }
        public string robotAddress { get; set; }
        public string userId { get; set; }
    }
}
