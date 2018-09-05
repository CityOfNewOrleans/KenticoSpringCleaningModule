using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace SpringCleaning
{

    public class AttachmentMover {

        protected static AttachmentMover instance;

        protected bool RunningInternal { get; set; }

        protected bool Cancelled { get; set; }

        protected List<string> ProgressMessageBuffer { get; set; }

        protected static readonly object padlock = new object();

        protected static Thread RunningThread { get; set; }

        protected static AttachmentMover Instance {
            get {
                lock (padlock)
                {
                    if (instance == null) instance = new AttachmentMover();

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

        protected AttachmentMover()
        {
            RunningInternal = false;
            ProgressMessageBuffer = new List<string>();
        }

        public static void Start(bool runFake = false)
        {
            if (runFake)
            {
                RunningThread = new Thread(new ThreadStart(() => instance.RunFake()))
                {
                    Name = "AttachmentMover"
                };

                RunningThread.Start();
                return;
            }

            RunningThread = new Thread(new ThreadStart(instance.RunInternal))
            {
                Name = "AttachmentMover"
            };

            RunningThread.Start();
        }

        public static void Stop()
        {
            try
            {
                if (RunningThread == null) return;

                Instance.Cancelled = true; 
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<string> DumpProgress()
        {
            var mb = Instance.ProgressMessageBuffer;

            var outVal = mb.GetRange(0, mb.Count);
            mb.RemoveRange(0, outVal.Count);

            return outVal;
        }

        protected void RunInternal() {
            try
            {
                RunningInternal = true;

                ProgressMessageBuffer.Add("Starting cleaning process...");

                var attachmentIDs = AttachmentInfoProvider
                    .GetAttachments(null, "AttachmentName", false)
                    .Select(att => att.AttachmentID);

                if (attachmentIDs == null) return;

                var sites = SiteInfoProvider.GetSites();

                // Configure attachment storage setting keys. Mover won't work without this:
                SettingsKeyInfoProvider.SetValue("CMSStoreFilesInFileSystem", "True", false);
                SettingsKeyInfoProvider.SetValue("CMSStoreFilesInDatabase", "False", false);

                foreach (var aID in attachmentIDs)
                {
                    if (Cancelled)
                    {
                        RunningInternal = false;
                        Cancelled = false;
                        return;
                    }

                    var att = AttachmentInfoProvider.GetAttachmentInfo(aID, false);

                    var attSite = sites.FirstOrDefault(s => s.SiteID == att.AttachmentSiteID);

                    if (attSite == null) continue;

                    AttachmentInfoProvider.EnsurePhysicalFile(att, attSite.SiteName);

                    att.AttachmentBinary = null;
                    att.Generalized.UpdateData();

                    ProgressMessageBuffer.Add(att.AttachmentName + " copied to file system.");
                }

                ProgressMessageBuffer.Add("Cleaning Process Complete");

                RunningInternal = false;
            }
            catch (Exception e)
            {
                ProgressMessageBuffer.Add("ERROR --------------------------");
                ProgressMessageBuffer.Add(e.Message);
                ProgressMessageBuffer.Add(e.StackTrace);
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

