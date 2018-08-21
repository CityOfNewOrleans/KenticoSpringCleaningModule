<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSModules_SpringCleaning_Default" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta charset="utf-8"/>
    <title>Spring Cleaning Control Panel</title>
    <style>
        [v-cloak] {
            display: none;
        }
    </style>
</head>
<body>
    <main id="controls" v-cloak>
        <div>

            <h1>Spring Cleaning</h1>
            <p>
                <span>Total attachments currently stored in database.</span>
                <span>{{pageModel.TotalAttachmentsInDB}}</span>
            </p>
            
            <p>
                <span>Attachments on this site currently stored in database:&nbsp;</span>
                <span>{{pageModel.SiteAttachmentsInDB}}</span>
            </p>
            <p>
                <span>Attachment Mover Is Running:&nbsp;</span>
                <span v-if="pageModel.MoverIsRunning">Yes</span>
                <span v-if="!pageModel.MoverIsRunning">No</span>
            </p>

            <button 
                v-if="!pageModel.MoverIsRunning"
                v-on:click.prevent="postStartCommand"
            >Start Cleaning</button>
            <button
                v-if="pageModel.MoverIsRunning"
                v-on:click.prevent="postStopCommand"
            >StopCleaning</button>

        </div>
    </main>

    <script src="https://cdn.jsdelivr.net/npm/vue/dist/vue.js"></script>
    <script>
        const pageModel = <%= js.Serialize(Model) %>;
    </script>
    <script>
        (() => {
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
                data: {
                    pageModel
                },
                methods: {
                    async postStartCommand() {
                        const resp = await postData("Default.aspx/StartCleaningProcess",{please: true});

                        if (!resp.Success) return;

                        this.pageModel.MoverIsRunning = resp.Running;
                    },
                    async postStopCommand() {
                        const resp = await postData("Default.aspx/StopCleaningProcess");

                        if (!resp.Success) return;

                        this.pageModel.MoverIsRunning = resp.Running;
                    }
                },
            });
        })();

    </script>

</body>
</html>
