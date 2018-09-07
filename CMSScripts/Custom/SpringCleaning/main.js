import Vue from "./vue.esm.browser.js";
import progressDialog from "./progressDialog.js";

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
        openAttachmentMoverDialog() {
            this.showAttachmentMoverProgress = true;
            this.getAttachmentMoverProgress();
        },
        closeAttachmentMoverDialog() {
            this.showAttachmentMoverProgress = false;

            if (!this.AttachmentMoverIsRunning)
                this.attachmentMoverProgress = "";
        },
        openAttachmentHistoryRemoverDialog() {
            this.showAttachmentHistoryRemoverProgress = true;
            this.getAttachmentHistoryRemoverProgress();
        },
        closeAttachmentHistoryRemoverDialog() {
            this.showAttachmentHistoryRemoverProgress = false;

            if (!this.AttachmentHistoryRemoverIsRunning)
            this.attachmentHistoryRemoverProgress = "";
        },
    },

});

window.vm = vm;

