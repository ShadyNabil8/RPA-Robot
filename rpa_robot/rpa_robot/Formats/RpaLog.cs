﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpa_robot.Formats
{
    internal class RpaLog
    {
        public string eventType { get; set; }
        public Payload payload { get; set; }
    }
}
