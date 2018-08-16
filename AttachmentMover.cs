namespace SpringCleaning
{

    public class AttachmentMover {

        public void MoveToFileSystem() {
            // Using the AttachmentManager class, get a dataset containing all attachments
            var aip = new AttachmentInfoProvider();
            var attachments = am.GetAttachments("", "", false);
        
            // Make sure the dataset is not null
            if (attachments == null) return;
                
            foreach (var dr in attachments.Tables[0].Rows)
            {  
                // Check that there is a valid GUID for the attachment
                var attachmentId = (Guid)dr["AttachmentGUID"];
                
                if (attachmentId == null) continue;

                // Create an AttachmentInfo object for this GUID
                var ai = am.GetAttachmentInfo(attachmentId, CMSContext.CurrentSiteName);
             
                // Confirm the AttachmentInfo exists for the current site
                if (ai == null) continue;
                 
                // Call EnsurePhysicalFile to create a file in the file system, if needed
                am.EnsurePhysicalFile(ai, CMSContext.CurrentSiteName);
            }
        }

    }

}

