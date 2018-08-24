const template = `
<dialog>
    <h4 class="header">Progress...</h4>
    <div class="progress">
        <p>{{progress}}</p>
    </div>
    <div class="footer">
        <button v-on:click="$emit('close')">Stop</button>
    </div>
</dialog>
<div class="dialog-backdrop"></div>
`;

const progressDialog = {
    template,
    props: {
        progress: {
            type: String,
            required: true,
        }
    },
    data() { return {

    };},
};

export default progressDialog;