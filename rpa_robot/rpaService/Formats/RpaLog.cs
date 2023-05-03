using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpaService.Formats
{
    internal class RpaLog
    {
        public string eventType { get; set; }
        public Payload payload { get; set; }
    }
}
