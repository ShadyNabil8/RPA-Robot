using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using rpaService.Formats;
using WebSocketSharp;
using Serilog;
using System.Threading.Tasks;
using Polly;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;

namespace rpaService.Classes
{
    internal class Orchestrator
    {

        public static Queue<string> OrchestratorProcessQueue = new Queue<string>();
        public static WebSocket ws = null; /* Authentication and logging ws */
        public static int userID = 0;
        private static int MaxRetryCount = 10;


        /// <summary>
        /// Initiates the authentication process by connecting the robot to the Orchestrator and establishing a WebSocket connection.
        /// The method follows a retry policy to handle exceptions and retries in case of connection failures or errors.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method is responsible for authenticating the robot by connecting it to the Orchestrator and establishing a WebSocket connection.
        /// It starts by serializing the robot's information (username and password) into JSON format.
        /// The method uses a retry policy to handle exceptions and retries in case of connection failures or errors. It will make multiple attempts
        /// to connect to the Orchestrator with an exponential backoff delay between each attempt.
        /// Once the connection is established, it sends a POST request to the specified URL with the serialized robot information.
        /// If the authentication is successful (status code 200 [OK]), it retrieves the response content and deserializes it into a Token object.
        /// It then creates a WebSocket connection URL using the obtained token and connects to the WebSocket asynchronously.
        /// The method waits for the WebSocket connection to be established before logging the successful connection.
        /// If the authentication is unsuccessful (status code other than 200 [OK]), it deserializes the response content into a Message object.
        /// </remarks> 
        public static async void MakeAuthenticationAsync()
        {
            // Serialize the robot information (username and password) into JSON format
            var robotInformation = JsonConvert.SerializeObject(new RobotInfo
            {
                username = Globals.RobotUsername,
                password = Globals.RobotPassword
            });

            //.HandleResult<bool>(result => result != true)
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: MaxRetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        // Log the retry attempt
                        Log.Information($"Authentication Ws:Retry attempt {retryCount}. Retrying in {timespan.TotalSeconds} seconds.");

                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                Token token = await GetToken();
                await ConnectToWs(token);
            });
        }


        /// <summary>
        /// Event handler for WebSocket errors.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ErrorEventArgs containing error information.</param>
        private static void WSOnError(object sender, ErrorEventArgs e)
        {
            // Log the WebSocket error message
            Log.Error(e.Message);
        }

        /// <summary>
        /// Event handler for WebSocket close event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The CloseEventArgs containing close event information.</param>
        private static void WSOnClose(object sender, CloseEventArgs e)
        {
            // Log that the WebSocket is closed and provide the reason
            Log.Error($"WebSocket is closed: : {e.Reason}");
        }

        /// <summary>
        /// Event handler for WebSocket incoming messages.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MessageEventArgs containing the incoming message data.</param>
        private static void WebSocketISR(object sender, MessageEventArgs e)
        {
            if (e.IsPing)
            {
                Log.Information("Ping Received!");
                Reconnect.loggingWspingReceived = true;
            }
            else
            {
                lock (OrchestratorProcessQueue)
                {
                    // Enqueue the incoming message data into the OrchestratorProcessQueue for further processing
                    OrchestratorProcessQueue.Enqueue(e.Data);
                }
            }
        }
        
        
        static async Task<Token> GetToken()
        {
            Token token = null;
            var robotInformation = JsonConvert.SerializeObject(new RobotInfo
            {
                username = Globals.RobotUsername,
                password = Globals.RobotPassword
            });
            Log.Information("Service is trying to connect to the Orchestrator");
            using (var client = new HttpClient())
            {
                HttpResponseMessage response;
                try
                {
                    // Create the content for the POST request with the serialized robot information
                    var content = new StringContent(robotInformation, Encoding.UTF8, "application/json");

                    // Send a POST request to the specified URL with the content and get the response
                    response = await client.PostAsync(Globals.AuthenticationEndPoint, content);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Log.Error("An error occurred during the POST request: Internet issue" + ex.Message);
                    // Handle the exception or rethrow it to trigger a retry
                    throw;
                }

                // Get the status code
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    // Authentication successful (status code 200 [OK])
                    // Log the successful status code
                    Log.Information("StatusCode 200[OK] is received!");

                    // Read the response content as a string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    try
                    {
                        // Deserialize the response content into a Token object
                        token = JsonConvert.DeserializeObject<Token>(responseContent);

                        // Log the obtained token
                        Log.Information($"The Token: {token.token}");
                    }
                    catch (Exception)
                    {

                        Log.Information("Orchws => Error in Deserializing the token");
                    }
                    //Log.Information($"userID: {token.userID}");
                    //userID = token.userID;
                }

                else
                {
                    // Authentication unsuccessful (status code other than 200 [OK])
                    // Deserialize the response content into a Message object
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Message msg = JsonConvert.DeserializeObject<Message>(responseContent);
                    Log.Information(msg.message);
                }
            }
            return token;
        }
        static async Task ConnectToWs(Token token)
        {
            ws = new WebSocket($"{Globals.WebSocketCreationEndPoint}{token.token}");

            // Subscribe to WebSocket events
            ws.OnMessage += WebSocketISR;
            ws.OnClose += WSOnClose;
            ws.OnError += WSOnError;
            ws.EmitOnPing = true;

            // Create a task completion source to track the completion of the connection
            var connectionTaskCompletionSource = new TaskCompletionSource<bool>();

            // Handle the Open event to complete the task when the connection is established
            ws.OnOpen += (sender, e) =>
            {
                connectionTaskCompletionSource.SetResult(true);
                Log.Information("logging Websocket is open");
            };

            // Log the initiation of the WebSocket connection
            Log.Information("Service is trying to connect to the logging WebSocket!");

            // Connect to the WebSocket asynchronously
            await Task.Run(() => ws.Connect());

            // Wait for the connection task to complete
            await connectionTaskCompletionSource.Task;


            // Log the successful WebSocket connection
            Log.Information("Service connected to the logging WebSocket!");

        }
    }

}
