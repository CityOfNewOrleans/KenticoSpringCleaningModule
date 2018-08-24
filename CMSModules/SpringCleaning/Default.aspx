<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSModules_SpringCleaning_Default" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8"/>
    <title>Spring Cleaning Control Panel</title>
    <link rel="stylesheet" href="css/main.css">
</head>
<body>
    <main id="controls" v-cloak>
        <div>

            <h1>Spring Cleaning</h1>
            <p>
                <span>Total attachments currently stored in database.</span>
                <span>{{TotalAttachmentsInDB}}</span>
            </p>
            
            <p>
                <span>Attachment Mover:&nbsp;</span>
                <span v-if="!AttachmentMoverIsRunning">Not&nbsp;</span>
                <span>Running</span>
            </p>
            
            <p>
                <span>Attachment History Remover:&nbsp;</span>
                <span v-if="!AttachmentHistoryRemoverIsRunning">Not</span>
                <span>Running</span>
            </p>

            <div>
                <div>Move Attachments To File System</div>
                <div>
                    <button 
                        v-disabled="AttachmentMoverIsRunning"
                        v-on:click.prevent="startAttachmentMover"
                    >Start</button>
                    <button
                        v-disabled="!AttachmentMoverIsRunning"
                        v-on:click.prevent="stopAttachmentMover"
                    >Stop</button>
                </div>
            </div>

            <progress-dialog
                v-if="AttachmentMoverIsRunning"
                :progress="attachmentMoverProgress"
                v-on:close="stopAttachmentMover"
            />

            <progress-dialog 
                v-if="AttachmentHistoryRemoverIsRunning"
                :progress="attachmentHistoryRemoverProgress"
                v-on:close="stopAttachmentHistoryRemover"
            />

        </div>
    </main>

    <script>
        const pageModel = <%= js.Serialize(Model) %>;
    </script>
    <script type="module" src="js/main.js"></script>

</body>
</html>
