using Newtonsoft.Json;
using rpa_robot.Formats;
using System;
using System.ComponentModel;
using Serilog;

namespace rpa_robot.Classes
{
    internal class Listener
    {
        public static void Listen(object sender, DoWorkEventArgs e) 
        {
            AsynchronousSocketListener.StartListening();
        }
        public static void DataReceivedHandler(string data)
        {
            bool deserializingDone = true;
            Data servicetMsg = null;
            try
            {
                servicetMsg = JsonConvert.DeserializeObject<Data>(data);
            }
            catch (Exception)
            {
                deserializingDone = false;
                Log.Error("Error in Deserializing the service msg!");
            }
            if (deserializingDone)
            {
                if (servicetMsg.eventType.Equals("uuid"))
                {
                    Globals.uuid = servicetMsg.payload;
                    Log.Information($"uuid is {servicetMsg.payload}");
                }
            }


        }
    }
}
