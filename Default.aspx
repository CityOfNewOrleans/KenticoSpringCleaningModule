<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="CMSModules_SpringCleaning_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="controls">
        <div>

            <h1>Spring Cleaning</h1>

            <button @click.prevent="postStartCommand">Start Cleaning</button>
            
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/vue/dist/vue.js"></script>
    
    <script>
        const vm = new Vue({
            el: "#controls",
            data: {},
            methods: {
                postStartCommand() {
                    console.log("sending start command...");
                }
            },
        });

        

    </script>

</body>
</html>
