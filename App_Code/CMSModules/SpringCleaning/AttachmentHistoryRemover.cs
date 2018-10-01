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

        public static void Start(int daysBeforeLastModified, int maxAllowedVersions, bool runFake = false)
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

            RunningThread = new Thread(new ThreadStart(() => instance.RunInternal(daysBeforeLastModified, maxAllowedVersions)))
            {
                Name = "AttachmentHistoryRemover"
            };

            RunningThread.Start();

        }

        public static void Stop()
        {
            if (Instance.RunningInternal) Instance.Cancelled = true;
        }

        protected void RunInternal(int daysBeforeLastModified, int maxAllowedVersions) {
            try
            {
                RunningInternal = true;

                ProgressMessageBuffer.Add("Starting attachment history removal process...");
                ProgressMessageBuffer.Add(string.Format(
                    "Limiting attachments older than {0} days to no more than {1} versions", 
                    daysBeforeLastModified, 
                    maxAllowedVersions
                ));

                var cutoffDate = DateTime.Today.AddDays(-daysBeforeLastModified);

                var cutoffDateString = string.Format("{0}-{1}-{2}", cutoffDate.Year, cutoffDate.Month + 1, cutoffDate.Day);

                var where = string.Format("AttachmentLastModified < '{0}'", cutoffDateString);

                ProgressMessageBuffer.Add(where);

                var attachments = AttachmentInfoProvider.GetAttachments(where, "AttachmentName", false, 0);

                ProgressMessageBuffer.Add(string.Format("Found {0} attachments last modified before {1}.", attachments.Count, cutoffDateString));

                foreach (var att in attachments)
                {
                    if (Cancelled) {
                        ProgressMessageBuffer.Add("Attachment history removal process cancelled.");
                        return;
                    }

                    TruncateAttachmentHistory(att, maxAllowedVersions);            

                    
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

        protected void TruncateAttachmentHistory(AttachmentInfo att, int maxAllowedVersions = 1, bool clearSingleHistoryBinaries = false)
        {
            try
            {
                // If we delete ALL attachment history, Kentico can't find ANY attacments:
                if (maxAllowedVersions < 1) return;

                var where = string.Format("AttachmentGuid = '{0}'", att.AttachmentGUID);

                var attachmentHistories = AttachmentHistoryInfoProvider
                    .GetAttachmentHistories(where, "AttachmentLastModified", 0, "AttachmentHistoryID")
                    .BinaryData(false);

                var attachmentHistoriesCount = attachmentHistories.Count();

                ProgressMessageBuffer.Add(string.Format("Found {0} histories for {1}", attachmentHistoriesCount, att.AttachmentName));

                // DON'T EVER DELETE ALL histories for an attachment. It will become unfindable to Kentico.
                if (attachmentHistoriesCount <= maxAllowedVersions || attachmentHistoriesCount == 1) return;

                var maxAllowedRange = attachmentHistoriesCount - 1;
                var range = (maxAllowedVersions > maxAllowedRange)
                    ? maxAllowedRange
                    : attachmentHistoriesCount - maxAllowedVersions;

                var historiesToDelete = attachmentHistories.Take(range);

                var historiesToDeleteCount = historiesToDelete.Count();

                if (historiesToDeleteCount == 0) return;

                if (attachmentHistoriesCount - historiesToDeleteCount < 1) return;

                foreach (var h in historiesToDelete)
                {
                    h.Generalized.DeleteData();
                    h.Generalized.UpdateData();
                }

                var finalHistoryCount = attachmentHistoriesCount - historiesToDeleteCount;

                ProgressMessageBuffer.Add(string.Format(
                    "Truncated history for {0} from {1} versions to {2}", 
                    att.AttachmentName, 
                    attachmentHistoriesCount, 
                    finalHistoryCount
                ));
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

