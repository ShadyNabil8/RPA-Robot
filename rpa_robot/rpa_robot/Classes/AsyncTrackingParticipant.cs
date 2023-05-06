using Newtonsoft.Json;
using rpa_robot.Classes;
using rpa_robot.Formats;
using Serilog;
using System;
using System.Activities.Tracking;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rpa_robot
{
    public class AsyncTrackingParticipant : TrackingParticipant
    {

        // A queue to store the tracking records
        private ConcurrentQueue<TrackingRecord> queue;

        // A background worker to process the records
        private BackgroundWorker worker;

        public AsyncTrackingParticipant()
        {
            // Initialize the queue
            queue = new ConcurrentQueue<TrackingRecord>();

            // Initialize and start the worker
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        protected override IAsyncResult BeginTrack(TrackingRecord record, TimeSpan timeout, AsyncCallback callback, object state)
        {
            // Enqueue the record
            queue.Enqueue(record);

            // Return a completed result
            return new CompletedAsyncResult(callback, state);
        }

        protected override void EndTrack(IAsyncResult result)
        {
            // Complete the result
            CompletedAsyncResult.End(result);
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Loop until cancelled
            while (!worker.CancellationPending)
            {
                // Dequeue a record if any
                if (queue.TryDequeue(out TrackingRecord record))
                {
                    
                    if (record is ActivityStateRecord activityStateRecord)
                    {


                        if (!activityStateRecord.Activity.Name.Equals("DynamicActivity"))
                        {
                           
                            //Globals.uiDispatcher.Invoke(() => {
                            //    Globals.StatusTxtBox.AppendText(JsonConvert.SerializeObject(new Activity
                            //    {
                            //        Type = "LogType",
                            //        Name = activityStateRecord.Activity.Name,
                            //        Status = activityStateRecord.State,
                            //        Time = activityStateRecord.EventTime.ToString()
                            //    })+"\n");
                            //});

                            var log = JsonConvert.SerializeObject(new RpaLog
                            {
                                eventType = "logEmitEvent",
                                payload = new Payload
                                { 
                                    logType = "ERROR",
                                    name = activityStateRecord.Activity.Name,
                                    status = activityStateRecord.State,
                                    timestamp = activityStateRecord.EventTime.ToString(),
                                    message = "this is a log entry",
                                    robotId = 1
                                }
                            });
                            //Globals.RobotAsyncClientFromService.SendToSocket(log);
                            lock (Handler.LogQueue) 
                            {
                                Handler.LogQueue.Enqueue(log);
                            }
                            

                        }
                        //Thread.Sleep(1000);
                    }





                }
                else
                {
                    // Wait for some time before trying again
                    Thread.Sleep(100);
                }
            }
        }
    }

    // A helper class for creating a completed IAsyncResult object
    public class CompletedAsyncResult : IAsyncResult
    {
        private AsyncCallback callback;
        private object state;

        public CompletedAsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            if (this.callback != null) this.callback(this);
        }

        public static void End(IAsyncResult result)
        {
            CompletedAsyncResult completedResult = result as CompletedAsyncResult;
            if (completedResult == null) throw new ArgumentException("Invalid async result.", "result");
        }

        public object AsyncState { get { return this.state; } }
        public WaitHandle AsyncWaitHandle { get { throw new NotImplementedException(); } }
        public bool CompletedSynchronously { get { return true; } }
        public bool IsCompleted { get { return true; } }
    }
}
