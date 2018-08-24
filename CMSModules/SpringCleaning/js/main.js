import Vue from "https://cdn.jsdelivr.net/npm/vue@2.5.17/dist/vue.esm.browser.js";
import progressDialog from "./progressDialog";

const postData = async (url, data) => {
    const resp = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify(data)
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
    },
    methods: {
        async startAttachmentMover() {
            const resp = await postData("Default.aspx/StartMovingAttachmentsToFileSystem");

            if (!resp.Success) return;

            this.AttachmentMoverIsRunning = resp.Running;
            
            this.getAttachmentMoverProgress();
        },
        async stopAttachmentMover() {
            const resp = await postData("Default.aspx/StopMovingAttachmentsToFileSystem");

            if (!resp.Success) return;

            this.AttachmentMoverIsRunning = resp.Running;
        },
        async getAttachmentMoverProgress() {
            const resp = await postData("Default.aspx/GetAttachmentMoverProgress");

            this.attachmentMoverProgress 
                = `${resp.Messages}${this.attachmentMoverProgress}`;

            if (resp.Running) this.getAttachmentMoverProgress();
        },
        async startAttachmentHistoryRemover() {
            const resp = await postData("Default.aspx/StartRemovingAttachmentHistory");

            if (!resp.Success) return console.log(resp.Error);

            this.AttachmentHistoryRemoverIsRunning = resp.Running;
        },
        async stopAttachmentHistoryRemover() {
            const resp = await postData("Default.aspx/StopRemovingAttachmentHistory");

            if (!resp.Success) return console.log(resp.Error);

            this.AttachmentHistoryRemoverIsRunning = resp.Running;
        },
        async getAttachmentHistoryRemoverProgress(){
            const resp = await postData("Default.aspx/GetAttachmentHistoryRemoverProgress");

            this.attachmentHistoryRemoverProgress 
                = `${resp.Messages}${this.attachmentHistoryRemoverProgress}`;

            if (resp.Running) this.getAttachmentHistoryRemoverProgress();
        },
    },
});
