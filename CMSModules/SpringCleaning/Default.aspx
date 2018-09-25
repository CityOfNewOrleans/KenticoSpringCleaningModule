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
                <span>Total attachments:</span>
                <span>{{TotalAttachments}}</span>
            </h3>
            <h3>
                <span>Total attachment histories:</span>
                <span>{{TotalAttachmentHistories}}</span>
            </h3>
            <h3>
                <span>Attachment Mover:&nbsp;</span>
                <span v-if="!AttachmentMoverIsRunning">Not&nbsp;</span>
                <span>Running</span>
                <button 
                    v-if="AttachmentMoverIsRunning"
                    v-on:click="openAttachmentMoverProgressDialog"
                >Show Progress</button>
            </h3>
            <h3>
                <span>Attachment History Remover:&nbsp;</span>
                <span v-if="!AttachmentHistoryRemoverIsRunning">Not</span>
                <span>Running</span>
                <button
                    v-if="AttachmentHistoryRemoverIsRunning"
                    v-on:click="openAttachmentHistoryRemoverProgressDialog"
                >Show Progress</button>
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
                v-on:close="closeAttachmentMoverProgressDialog"
            ></progress-dialog>

            <progress-dialog 
                v-if="showAttachmentHistoryRemoverProgress"
                :title="'Attachment History Remover'"
                :progress="attachmentHistoryRemoverProgress"
                v-on:stop="stopAttachmentHistoryRemover"
                v-on:close="closeAttachmentHistoryRemoverProgressDialog"
            ></progress-dialog>

        </div>
    </main>

    <script>
        const pageModel = <%= js.Serialize(Model) %>;
    </script>
    <!-- <script type="module" src="/CMSScripts/Custom/SpringCleaning/main.js"></script> -->
    <script src="https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.js"></script>
    <script>



      const progressTemplate = `
<div class="dialog-backdrop">
<dialog>
    <h3 class="header">{{title}} Progress...</h3>
    <div class="progress">
        <p>{{progress}}</p>
    </div>
    <div class="footer">
        <button v-on:click="$emit('close')">Close</button>
        <button v-on:click="$emit('stop')">Stop</button>
    </div>
</dialog>
</div>
`;

const progressDialog = {
    template: progressTemplate,
    props: {
        title: {
            type: String,
            required: true,
        },
        progress: {
            type: String,
            required: true,
        },
    },
    data() { return {

    };},
    methods: {
        invokeStopCallback () {
            this.stopCallback();
        }
    }

};  

        const parameterize = (data) => (data)
    ? "?" + Object.keys(data).map((k) => `${k}=${encodeURIComponent(data[k])}`)
    : "";

    const postData = async (url, data) => {
        const resp = await fetch(url + parameterize(data), {
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
    });

    const json = await resp.json();

    try {
        return JSON.parse(json.d);
    }
    catch (e) {
        return json.d;
    }
};

const vm = new Vue({
    el: "#controls",
    components: {
        "progress-dialog": progressDialog, 
    },
    data: {
        ...pageModel,
        attachmentMoverProgress: "",
        attachmentHistoryRemoverProgress: "",
        showAttachmentMoverProgress: false,
        showAttachmentHistoryRemoverProgress: false,
    },
    methods: {
        async startAttachmentMover() {
            const resp = await postData("Default.aspx/StartMovingAttachmentsToFileSystem");

            if (!resp.Success) return;

            this.showAttachmentMoverProgress = true;

            this.attachmentMoverProgress = "";

            this.getAttachmentMoverProgress();
        },
        async stopAttachmentMover() {
            const resp = await postData("Default.aspx/StopMovingAttachmentsToFileSystem");

            if (!resp.Success) return;

            this.AttachmentMoverIsRunning = resp.Running;
        },
        async getAttachmentMoverProgress(retries = 0) {
            const resp = await postData("Default.aspx/GetAttachmentMoverProgress");

            this.attachmentMoverProgress 
                = `${resp.Messages}${this.attachmentMoverProgress}`;

            if (resp.Running) this.getAttachmentMoverProgress();

            if (!resp.Running && retries < 3)
                setInterval(() => {
                    this.getAttachmentMoverProgress(retries++);
                }, 5000);
        },
        async startAttachmentHistoryRemover() {
            console.log("start");

            const resp = await postData("Default.aspx/StartRemovingAttachmentHistory");

            if (!resp.Success) return console.log(resp.Error);

            this.AttachmentHistoryRemoverIsRunning = resp.Running;

            this.attachmentHistoryRemoverProgress = "";

            this.showAttachmentHistoryRemoverProgress = true;

            this.getAttachmentHistoryRemoverProgress();
        },
        async stopAttachmentHistoryRemover() {
            const resp = await postData("Default.aspx/StopRemovingAttachmentHistory");

            if (!resp.Success) return console.log(resp.Error);

            this.AttachmentHistoryRemoverIsRunning = resp.Running;
        },
        async getAttachmentHistoryRemoverProgress(retries = 0){
            const resp = await postData("Default.aspx/GetAttachmentHistoryRemoverProgress");

            this.attachmentHistoryRemoverProgress 
                = `${resp.Messages}${this.attachmentHistoryRemoverProgress}`;

            if (resp.Running) this.getAttachmentHistoryRemoverProgress();

            if (!resp.Running && retries < 3)
                setInterval(() => {
                    this.getAttachmentHistoryRemoverProgress(retries++);
                }, 5000);
        },
        openAttachmentMoverProgressDialog() {
            this.showAttachmentMoverProgress = true;
            this.getAttachmentMoverProgress();
        },
        closeAttachmentMoverProgressDialog() {
            this.showAttachmentMoverProgress = false;

            if (!this.AttachmentMoverIsRunning)
                this.attachmentMoverProgress = "";
        },
        openAttachmentHistoryRemoverProgressDialog() {
            this.showAttachmentHistoryRemoverProgress = true;
            this.getAttachmentHistoryRemoverProgress();
        },
        closeAttachmentHistoryRemoverProgressDialog() {
            this.showAttachmentHistoryRemoverProgress = false;

            if (!this.AttachmentHistoryRemoverIsRunning)
            this.attachmentHistoryRemoverProgress = "";
        },
    },

});

window.vm = vm;

    </script>
</body>
</html>
