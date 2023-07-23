using Newtonsoft.Json;
using rpa_robot.Classes;
using rpa_robot.Formats;
using System;
using System.Activities.Tracking;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Serilog;

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
                            //Helper.PrintOnUI($"Activity name: {activityStateRecord.Activity.Name}\nStatus: {activityStateRecord.State}\nTime: {activityStateRecord.EventTime}\n--------------------\n");
                            
                            var log = JsonConvert.SerializeObject(new Data
                            {
                                eventType = "logEmitEvent",
                                payload = JsonConvert.SerializeObject(new Payload
                                {
                                    logType = "Info",
                                    name = activityStateRecord.Activity.Name,
                                    status = activityStateRecord.State,
                                    timestamp = Helper.GetTime(),
                                    message = "this is a log entry",
                                    robotAddress = Helper.ReadRobotAd(),
                                    userId = Helper.ReadUserID()
                                })
                            });
                            Log.Information(log);
                            //Helper.SendToService(log);
                        }
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
