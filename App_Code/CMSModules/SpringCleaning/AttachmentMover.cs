using CMS.DocumentEngine;
using CMS.SiteProvider;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace SpringCleaning
{

    public class AttachmentMover {

        protected static AttachmentMover instance;

        protected bool running { get; set; }

        protected List<string> messageBuffer { get; set; }

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
                return Instance.running;
            }
            protected set
            {

            } 
        }

        protected AttachmentMover()
        {
            running = false;
            messageBuffer = new List<string>();
        }

        public static void Run()
        {
            RunningThread = new Thread(new ThreadStart(instance.RunInternal));
            RunningThread.Name = "AttachmentMover";

            RunningThread.Start();
        }

        public static void Stop()
        {
            try
            {
                if (RunningThread != null) RunningThread.Abort();
            }
            catch (Exception e)
            {

            }
        }

        protected void RunInternal() {
            try
            {
                messageBuffer.Append("Starting cleaning process...");

                var attachments = AttachmentInfoProvider.GetAttachments(null, null, false);

                if (attachments == null) return;

                running = true;

                var sites = SiteInfoProvider.GetSites();

                foreach (var att in attachments)
                {
                    var attSite = sites.FirstOrDefault(s => s.SiteID == att.AttachmentSiteID);

                    if (attSite == null) continue;

                    AttachmentInfoProvider.EnsurePhysicalFile(att, attSite.SiteName);
                    AttachmentInfoProvider.DeleteAttachmentInfo(att, false);

                    messageBuffer.Append("Moved " + att.AttachmentName + " to file system");
                }

                messageBuffer.Append("Cleaning Process Complete");

                running = false;
            }
            catch (Exception e)
            {
                messageBuffer.Append("ERROR --------------------------");
                messageBuffer.Append(e.Message);
                messageBuffer.Append(e.StackTrace);
                running = false;
            }
        }
    }

}

