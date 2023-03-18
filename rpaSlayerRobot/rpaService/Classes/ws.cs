using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace rpaService
{
    public class ws
    {
        private static WebSocket ws1;
        public static void WS_Connect()
        {
            ws1 = new WebSocket("wss://mountain-spot-cicada.glitch.me");
            ws1.OnMessage += Ws_OnMessage;
            try
            {
                ws1.Connect();
                Log.Information("Connected to the WebSocket server");
            }
            catch (Exception)
            {
                Log.Error("Error while connecting to the WebSocket server");
            }
            
        }

        // This function will be executed each time there is an incomming message
        private static void Ws_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            Log.Information("received from WS server: "+e.Data);
        }

        private static void WS_Send(String str)
        {
            ws1.Send(str);
        }
    }
}
