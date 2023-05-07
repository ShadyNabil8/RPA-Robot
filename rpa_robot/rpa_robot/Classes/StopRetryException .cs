using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rpa_robot.Classes
{
    public class StopRetryException : Exception
    {
        public bool StopRetry { get; }

        public StopRetryException(bool stopRetry = true)
        {
            StopRetry = stopRetry;
        }
    }

}
