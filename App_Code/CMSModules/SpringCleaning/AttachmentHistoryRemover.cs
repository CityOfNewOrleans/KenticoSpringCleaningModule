using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SpringCleaning
{

    public class AttachmentHistoryRemover {

        protected static AttachmentHistoryRemover instance;

        protected bool RunningInternal { get; set; }

        protected bool Cancelled { get; set;}

        protected List<string> ProgressMessageBuffer { get; set; }

        protected static readonly object padlock = new object();

        protected static Thread RunningThread { get; set; }

        protected static AttachmentHistoryRemover Instance {
            get {
                lock (padlock)
                {
                    if (instance == null) instance = new AttachmentHistoryRemover();

                    return instance;
                }
            }
        }

        public static bool Running {
            get
            {
                return Instance.RunningInternal;
            }
            protected set
            {

            } 
        }

        protected AttachmentHistoryRemover()
        {
            RunningInternal = false;
            ProgressMessageBuffer = new List<string>();
        }

        public static List<string> DumpProgress() {
            var mb = Instance.ProgressMessageBuffer;
            
            var outVal = mb.GetRange(0, mb.Count);
            mb.RemoveRange(0, outVal.Count);

            return outVal;
        }

        public static void Start(bool runFake = false)
        {
            if (runFake)
            {
                RunningThread = new Thread(new ThreadStart(() => instance.RunFake()))
                {
                    Name = "AttachmentHistoryRemover"
                };

                RunningThread.Start();

                return;
            }

            RunningThread = new Thread(new ThreadStart(instance.RunInternal))
            {
                Name = "AttachmentHistoryRemover"
            };

            RunningThread.Start();

        }

        public static void Stop()
        {
            if (Instance.RunningInternal) Instance.Cancelled = true;
        }

        protected void RunInternal() {
            try
            {
                RunningInternal = true;

                ProgressMessageBuffer.Add("Starting attachment history removal process...");

                var attachmentHistories = AttachmentHistoryInfoProvider
                    .GetAttachmentHistories(null, "AttachmentName", 0, "AttachmentHistoryID, AttachmentName");

                if (attachmentHistories == null) return;

                foreach (var ah in attachmentHistories)
                {
                    if (Cancelled) {
                        ProgressMessageBuffer.Add("Attachment history removal process cancelled.");
                        return;
                    }

                    AttachmentHistoryInfoProvider.DeleteAttachmentHistory(ah.AttachmentHistoryID);

                    ProgressMessageBuffer.Add("Removed " + ah.AttachmentName + " from db.");
                }

                ProgressMessageBuffer.Add("Attachment history removal process complete.");

                RunningInternal = false;
            }
            catch (Exception e)
            {
                ProgressMessageBuffer.Add("Removal stopped after encountering error.");
                ProgressMessageBuffer.Add(e.StackTrace);
                ProgressMessageBuffer.Add(e.Message);
                ProgressMessageBuffer.Add("ERROR --------------------------");

                EventLogProvider.LogEvent(new EventLogInfo("SPRING CLEANING", e.StackTrace, "REMOVE ATTACHMENT HISTORY"));

                RunningInternal = false;
            }
        }

        protected void RunFake(int iterations = 1000, int sleep = 10)
        {
            RunningInternal = true;

            ProgressMessageBuffer.Add("Starting fake run...");

            for (var i = 0; i < iterations; i++)
            {
                if (Cancelled)
                {
                    RunningInternal = false;
                    Cancelled = false;
                    ProgressMessageBuffer.Add("Stopping fake run at cancellation request");
                    return;
                }

                ProgressMessageBuffer.Add("Fake iteration " + i);

                Thread.Sleep(sleep);
            }

            ProgressMessageBuffer.Add("Fake run completed.");

            RunningInternal = false;
        }
    }

}

