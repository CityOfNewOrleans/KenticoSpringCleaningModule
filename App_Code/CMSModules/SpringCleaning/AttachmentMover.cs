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
        }

        public static void Run()
        {
            //RunningThread = new Thread(new ThreadStart(instance.RunInternal));
            //RunningThread.Name = "Attachment Mover";

            //RunningThread.Start();

            Instance.RunInternal();
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
            using (var sw = File.AppendText(HttpRuntime.AppDomainAppPath + "App_Code/CMSModules/SpringCleaning/log.txt"))
            {
                try
                {
                    sw.WriteLine("Starting cleaning process...");

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

                        sw.WriteLine("Moved " + att.AttachmentName + " to file system");
                    }

                    sw.WriteLine("Cleaning Process Complete");

                    running = false;
                }
                catch (Exception e)
                {
                    sw.WriteLine("ERROR --------------------------");
                    sw.WriteLine(e.Message);
                    sw.WriteLine(e.StackTrace);
                    running = false;
                }
            }
        }
    }

}

