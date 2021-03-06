using CMS.UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using CMS.DocumentEngine;
using SpringCleaning;
using CMS.SiteProvider;

public partial class CMSModules_SpringCleaning_Default : CMSPage
{
    public class PageModel
    {
        public int TotalAttachments { get; set; }
        public int TotalAttachmentHistories { get; set; }
        public int AttachmentsInFileSystem { get; set; }
        public bool AttachmentMoverIsRunning { get; set; }
        public bool AttachmentHistoryRemoverIsRunning { get; set; }
    }

    public static JavaScriptSerializer js { get; set; }

    public PageModel Model { get; set; }

    public CMSModules_SpringCleaning_Default()
    {
        js = new JavaScriptSerializer();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Model = GetPageModel();
    }

    protected PageModel GetPageModel ()
    {
        return new PageModel
        {
            TotalAttachments = AttachmentInfoProvider.GetCount(),
            TotalAttachmentHistories = AttachmentHistoryInfoProvider.GetCount(),
            AttachmentHistoryRemoverIsRunning = AttachmentHistoryRemover.Running,
            AttachmentMoverIsRunning = AttachmentMover.Running,
        };
    }

    [WebMethod]
    public static string StartMovingAttachmentsToFileSystem() {
        
        try {
            //AttachmentMover.Start(runFake: true);
            AttachmentMover.Start();

            return js.Serialize(new {
                Success = true,
                AttachmentMover.Running,
            });
        }
        catch (Exception e) {
            return js.Serialize(new {
                Success = false,
                e.Message,
            });
        }
    }

    [WebMethod]
    public static string StopMovingAttachmentsToFileSystem() {

        try
        {
            AttachmentMover.Stop();

            return js.Serialize(new { Success = true });
        }
        catch (Exception e)
        {
            return js.Serialize(new {
                Success = false,
                e.Message,
            });
        }

    }

    [WebMethod]
    public static string GetAttachmentMoverProgress() {

        var messages = string.Join("\n", AttachmentMover.DumpProgress().ToArray().Reverse()) + "\n";

        var progress = new {
            AttachmentMover.Running,
            Messages = string.IsNullOrWhiteSpace(messages) ? string.Empty : messages,
        };

        return js.Serialize(progress);
    }

    [WebMethod]
    public static string StartRemovingAttachmentHistory(int daysBeforeLastModified, int maxAllowedVersions) 
    {
        try
        {
            AttachmentHistoryRemover.Start(daysBeforeLastModified, maxAllowedVersions);

            return js.Serialize(new
            {
                Success = true,
                Running = true,
            });
        }
        catch (Exception e)
        {
            return js.Serialize(new {
                Success = false,
                e.Message,
            });
        }
        
    }

    [WebMethod]
    public static string StopRemovingAttachmentHistory()
    {
        AttachmentHistoryRemover.Stop();

        return js.Serialize(new
        {
            Success = true,
            AttachmentHistoryRemover.Running,
        });

    }

    [WebMethod]
    public static string GetAttachmentHistoryRemoverProgress()
    {
        var messages = string.Join("\n", AttachmentHistoryRemover.DumpProgress().ToArray().Reverse()) + "\n";

        var progress = new
        {
            AttachmentHistoryRemover.Running,
            Messages = string.IsNullOrWhiteSpace(messages) ? string.Empty : messages,
        };

        return js.Serialize(progress);
    }
    
}