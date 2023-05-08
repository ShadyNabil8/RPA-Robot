using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;
using rpa_robot.Formats;
using System.Threading;
using Polly;

namespace rpa_robot.Classes
{
    internal class Orchestrator
    {
        public static Queue<string> OrchestratorProcessQueue = new Queue<string>();
        public static WebSocket ws;
        public static bool Connected = false;
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
        public static async Task MakeAuthenticationAsync()
        {
            // Log the authentication process initiation
            Log.Information("Robot is trying to connect to the Orchestrator");
            Globals.LogsTxtBox.AppendText("Robot is trying to connect to the Orchestrator\n");

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
                        Log.Information($"Retry attempt {retryCount}. Retrying in {timespan.TotalSeconds} seconds.");
                        Globals.uiDispatcher.Invoke(() =>
                        {
                            Globals.LogsTxtBox.AppendText($"Retry attempt {retryCount}. Retrying in {timespan.TotalSeconds} seconds.\n");
                        });
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                Log.Information("Robot is trying to connect to the Orchestrator");
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
                        Log.Error($"An error occurred during the POST request: {ex}");
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
                        Globals.uiDispatcher.Invoke(() =>
                        {
                            Globals.LogsTxtBox.AppendText("StatusCode 200[OK] is received!\n");
                        });

                        // Read the response content as a string
                        var responseContent = await response.Content.ReadAsStringAsync();

                        // Deserialize the response content into a Token object
                        Token token = JsonConvert.DeserializeObject<Token>(responseContent);

                        // Create a WebSocket connection URL with the obtained token
                        ws = new WebSocket($"{Globals.WebSocketCreationEndPoint}{token.token}");

                        // Log the obtained token
                        Log.Information($"The Token: {token.token}");
                        Globals.uiDispatcher.Invoke(() =>
                        {
                            Globals.LogsTxtBox.AppendText($"The Token: {token.token}\n");
                        });

                        // Subscribe to WebSocket events
                        ws.OnMessage += WebSocketISR;
                        ws.OnClose += WSOnClose;
                        ws.OnError += WSOnError;
                        //ws.OnOpen += WSOnOpen;

                        // Create a task completion source to track the completion of the connection
                        var connectionTaskCompletionSource = new TaskCompletionSource<bool>();

                        // Handle the Open event to complete the task when the connection is established
                        ws.OnOpen += (sender, e) =>
                        {
                            connectionTaskCompletionSource.SetResult(true);
                            Log.Information("Websocket is open");
                        };

                        // Log the initiation of the WebSocket connection
                        Log.Information("Robot is trying to connect to the WebSocket!");
                        Globals.uiDispatcher.Invoke(() =>
                        {
                            Globals.LogsTxtBox.AppendText("Robot is trying to connect to the WebSocket!\n");
                        });

                        // Connect to the WebSocket asynchronously
                        await Task.Run(() => ws.Connect());

                        // Wait for the connection task to complete
                        await connectionTaskCompletionSource.Task;


                        // Log the successful WebSocket connection
                        Log.Information("Robot connected to the WebSocket!");
                        Globals.uiDispatcher.Invoke(() =>
                        {
                            Globals.LogsTxtBox.AppendText("Robot connected to the WebSocket!\n");
                        });
                    }

                    else
                    {
                        // Authentication unsuccessful (status code other than 200 [OK])
                        // Deserialize the response content into a Message object
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Message msg = JsonConvert.DeserializeObject<Message>(responseContent);
                    }
                }
            });
        }

        private static void WSOnError(object sender, ErrorEventArgs e)
        {
            Log.Information(e.Message);
        }

        private static void WSOnClose(object sender, CloseEventArgs e)
        {
            Log.Information(e.Reason);
        }

        private static void WebSocketISR(object sender, MessageEventArgs e)
        {
            lock (OrchestratorProcessQueue)
            {
                OrchestratorProcessQueue.Enqueue(e.Data);
            }
        }
    }
}
