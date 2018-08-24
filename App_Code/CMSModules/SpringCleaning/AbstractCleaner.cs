

namespace SpringCleaning
{
    public abstract class AbstractCleaner<T> 
        where T : new(), 
    {
        protected static T instance;

        protected bool running { get; set; }

        protected bool cancelled { get; set;}

        protected List<String> progressMessageBuffer { get; set; }

        protected static readonly object padlock = new object();

        protected static Thread RunningThread { get; set; }

        protected static T Instance {
            get {
                lock (padlock)
                {
                    if (instance == null) instance = new T();

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

        protected AttachmentHistoryRemover()
        {
            running = false;
            progressMessageBuffer = new List<string>();
        }

        public static List<string> DumpProgress() {
            var mb = Instance.progressMessageBuffer;
            
            var outVal = mb.GetRange(0, mb.Count);
            mb.RemoveRange(0, outVal.Count);

            return outVal;
        }

        public static void Run()
        {
            RunningThread = new Thread(new ThreadStart(instance.RunInternal));
            RunningThread.Name = "AttachmentHistoryRemover";

            RunningThread.Start();

        }

        public static void Stop()
        {
            if (Instance.running) Instance.cancelled = true;
        }

        protected void RunInternal() {
            try
            {
                progressMessageBuffer.Append("Starting attachment history removal process...");

                var attachmentHistories = AttachmentHistoryInfoProvider.GetattachmentHistories();

                if (attachmentHistories == null) return;

                running = true;

                foreach (var ah in attachmentHistories)
                {
                    if (cancelled) {
                        progressMessageBuffer.Append("Attachment history removal process cancelled.");
                        return;
                    }

                    AttachmentHistoryInfoProvider.DeleteAttachmentHistory(ah);

                    progressMessageBuffer.Append("Removed " + ah.AttachmentName + " from db.");
                }

                progressMessageBuffer.Append("Attachment history removal process complete.");

                running = false;
            }
            catch (Exception e)
            {
                progressMessageBuffer.Append("ERROR --------------------------");
                progressMessageBuffer.Append(e.Message);
                progressMessageBuffer.Append(e.StackTrace);
                running = false;
            }
        }
    }
}