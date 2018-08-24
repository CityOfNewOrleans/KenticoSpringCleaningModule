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
        public int TotalAttachmentsInDB { get; set; }
        public int SiteAttachmentsInDB { get; set; }
        public int AttachmentsInFileSystem { get; set; }
        public bool AttachmentMoverIsRunning { get; set; }
        public bool AttachmentHistoryRemoverIsRunning { get; set; }
    }

    public class StartCleaningProcessModel
    {
        public bool please { get; set; }
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
            TotalAttachmentsInDB = AttachmentInfoProvider.GetCount(),
            AttachmentHistoryRemoverIsRunning = AttachmentHistoryRemover.Running,
            AttachmentMoverIsRunning = AttachmentMover.Running,
        };
    }

    [WebMethod]
    public static string StartMovingAttachmentsToFileSystem() {
        
        try {
            AttachmentMover.Start();
            return js.Serialize(new {
                Success = true,
                Running = 
            });
        }
        catch (Exception e) {
            
        }
    }

    [WebMethod]
    public static string StopMovingAttachmentsToFileSystem() {

    }

    [WebMethod]
    public static string GetAttachmentMoverProgress() {
        
        var messages = 

        var progress = new {
            Running = AttachmentMover.Running,
            Messages = String.Join(System.Environment.NewLine, AttachmentMover.DumpProgress().ToArray());
        };

        return js.Serialize(progress);
    }

    [WebMethod]
    public static string StartRemovingAttachmentHistory(bool please) 
    {
        if (!please)
            return js.Serialize(new {
                Success = false,
                Message = "You didn't say the magic word...",
            });

        AttachmentHistoryRemover.Run();

        return js.Serialize(new {
            Sucess = true,
            AttachmentHistoryRemover.Running
        });
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

    
}