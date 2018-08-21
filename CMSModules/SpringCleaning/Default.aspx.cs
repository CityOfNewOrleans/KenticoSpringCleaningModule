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
        public bool MoverIsRunning { get; set; }
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
            SiteAttachmentsInDB = AttachmentInfoProvider
                .GetAttachments(null, null, false)
                .Count(a => a.AttachmentSiteID == SiteContext.CurrentSiteID),
            MoverIsRunning = AttachmentMover.Running,
        };
    }

    [WebMethod]
    public static string StartCleaningProcess(bool please) 
    {
        if (!please)
            return js.Serialize(new {
                Success = false,
                Message = "You didn't say the magic word...",
            });

        AttachmentMover.Run();

        return js.Serialize(new {
            Sucess = true,
            AttachmentMover.Running
        });
    }

    [WebMethod]
    public static string StopCleaningProcess()
    {
        AttachmentMover.Stop();

        return js.Serialize(new
        {
            Success = true,
            AttachmentMover.Running,
        });

    }

    
}