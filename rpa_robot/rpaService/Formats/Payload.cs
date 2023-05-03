using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpaService.Formats
{
    internal class Payload
    {
        public string logType { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string timestamp { get; set; }
        public string message { get; set; }
        public int robotId { get; set; }
    }
}
