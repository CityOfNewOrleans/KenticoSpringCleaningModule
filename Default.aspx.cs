using CMS.UIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

public partial class CMSModules_SpringCleaning_Default : CMSPage
{
    public class PageModel
    {
        public int AttachmentsInDB { get; set; }
        public int AttachmentsInFileSystem { get; set; }
    }

    public class StartCleaningProcessModel
    {
        public bool please { get; set; }
    }

    public JavaScriptSerializer js { get; set; }

    public PageModel Model { get; set; }

    public CMSModules_SpringCleaning_Default()
    {
        js = new JavaScriptSerializer();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    [WebMethod]
    public string StartCleaningProcess(string dtoInString) 
    {
        var dtoIn = js.Deserialize<StartCleaningProcessModel>(dtoInString);

        if (!dtoIn.please) return js.Serialize(new { error = true, message = "You didn't say the magic word..."});

        var dtoOut = new { };

        return js.Serialize(dtoOut);
    }

    
}