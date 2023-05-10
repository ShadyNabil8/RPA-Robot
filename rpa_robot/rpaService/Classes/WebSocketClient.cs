using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using rpaService.Formats;
using Serilog;

namespace rpaService.Classes
{
    internal class WebSocketClient
    {
        //private const string WebSocketUrl = "wss://your-websocket-url.com"; // Replace with your WebSocket URL
         /*
        private static ClientWebSocket webSocket;

        public static async Task ConnectAsync(string token)
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri($"{Globals.WebSocketCreationEndPoint}{token}"), CancellationToken.None);
            Log.Information("WebSocket connected!");

            // Start a separate task to receive messages
            _ = ReceiveMessagesAsync();
            //Globals.ListenerFromOrch.Start();
        }

        public static async Task SendMessageAsync(string message)
        {
            if (webSocket.State != WebSocketState.Open)
            {
                Log.Information("WebSocket is not connected.");
                return;
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            var sendMessageBuffer = new ArraySegment<byte>(messageBytes);

            await webSocket.SendAsync(sendMessageBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            Log.Information("Message sent to WebSocket!");
        }

        public static async Task ReceiveMessagesAsync()
        {
            if (webSocket == null || webSocket.State != WebSocketState.Open)
            {
                Log.Information("WebSocket is not connected or has been closed.");
                //return;
            }
            else 
            {
                byte[] receiveBuffer = new byte[1024];
                var receiveMessageBuffer = new ArraySegment<byte>(receiveBuffer);

                try
                {
                    while (webSocket.State == WebSocketState.Open)
                    {
                        WebSocketReceiveResult result = await webSocket.ReceiveAsync(receiveMessageBuffer, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                            Log.Information("Received message from WebSocket: " + receivedMessage);
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            Log.Information("WebSocket connection closed by the server.");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Information("Error while receiving messages: " + ex.Message);
                }
            }            
        }

        public static async Task DisconnectAsync()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
            }
        }
         */
    }
}
