const template = `
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
    template,
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

export default progressDialog;