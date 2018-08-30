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

            <h3>
                <span>Total attachments currently stored in database:</span>
                <span>{{TotalAttachmentsInDB}}</span>
            </h3>
            <h3>
                <span>Total attachments currently stored in file system:</span>
                <span>{{TotalAttachmentsInFileSystem}}</span>
            </h3>
            <h3>
                <span>Total attachment histories currently stored in database:</span>
                <span>{{TotalAttachmentHistoriesInDB}}</span>
            </h3>
            <h3>
                <span>Attachment Mover:&nbsp;</span>
                <span v-if="!AttachmentMoverIsRunning">Not&nbsp;</span>
                <span>Running</span>
            </h3>
            <h3>
                <span>Attachment History Remover:&nbsp;</span>
                <span v-if="!AttachmentHistoryRemoverIsRunning">Not</span>
                <span>Running</span>
            </h3>

            <div class="control">
                <h4>Move Attachments To File System</h4>
                <div>
                    <button 
                        :disabled="AttachmentMoverIsRunning"
                        v-on:click.prevent="startAttachmentMover"
                    >Start</button>
                    <button
                        :disabled="!AttachmentMoverIsRunning"
                        v-on:click.prevent="stopAttachmentMover"
                    >Stop</button>
                </div>
            </div>

            <div class="control">
                <h4>Remove Attachment History</h4>
                <div>
                    <button
                        :disabled="AttachmentHistoryRemoverIsRunning"
                        v-on:click.prevent="startAttachmentHistoryRemover"
                    >Start</button>
                    <button
                        :disabled="!AttachmentHistoryRemoverIsRunning"
                        v-on:click.prevent="stopAttachmentHistoryRemover"
                    >Stop</button>
                </div>
            </div>

            <progress-dialog
                v-if="showAttachmentMoverProgress"
                :title="'Attachment Mover'"
                :progress="attachmentMoverProgress"
                v-on:stop="stopAttachmentMover"
                v-on:close="showAttachmentMoverProgress = false"
            ></progress-dialog>

            <progress-dialog 
                v-if="showAttachmentHistoryRemoverProgress"
                :title="'Attachment History Remover'"
                :progress="attachmentHistoryRemoverProgress"
                v-on:stop="stopAttachmentHistoryRemover"
                v-on:close="showAttachmentHistoryRemoverProgress = false"
            ></progress-dialog>

        </div>
    </main>

    <script>
        const pageModel = <%= js.Serialize(Model) %>;
    </script>
    <script type="module" src="/CMSScripts/Custom/SpringCleaning/main.js"></script>

</body>
</html>
