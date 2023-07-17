

<script setup lang="ts">
    import { computed, ref } from "vue";


    const original = ref("");
    const matcherItems = ref([{
        regex: '',
        replaceStr: ''
    }]);

    const preview = computed(() => {
        let str = original.value;

        for (var i = 0; i < matcherItems.value.length; i++) {
            const item = matcherItems.value[i];

            if (item.regex && item.replaceStr) {
                const regex = new RegExp(item.regex, 'ig')
                str = str.replace(regex, `<span class='b-${i}'>${item.replaceStr}</span>`);
            }
        }
        return str;
    });

    const output = computed(() => {
        let str = original.value;

        for (var i = 0; i < matcherItems.value.length; i++) {
            const item = matcherItems.value[i];

            if (item.regex && item.replaceStr) {
                const regex = new RegExp(item.regex, 'ig')
                str = str.replace(regex, `<span class='b-${i}'>${item.replaceStr}</span>`);
            }
        }
        return str;
    });

    const addItem = () => {
        matcherItems.value.push({ regex: '', replaceStr: '' });
    }

    const removeItem = (index) => {
        matcherItems.value.splice(index, 1);
    }



</script>

<template>
    <div class="preview">
        <div class="pre-input">
            <el-input type="textarea" v-model="original" :rows="30"></el-input>
        </div>

        <div class="pre-regex-box">
            <div class="pre-matcher-item" v-for="(x ,index) in matcherItems">
                <el-input v-model="x.regex"></el-input>
                <div class="center">- <a @click="removeItem(index)">移除</a></div>
                <el-input v-model="x.replaceStr"></el-input>
            </div>

            <a @click="addItem">添加</a>
        </div>

        <div class="pre-view div-textarea" contenteditable="true" onkeydown="return false;" onpaste="return false;" v-html="preview">

        </div>
    </div>

    <!--<el-divider>最终输出</el-divider>

    <div class="output">
        <div class="div-textarea" v-html="output">
        </div>
    </div>-->

</template>


<style>
    .center {
        text-align: center;
    }

    .preview {
        display: flex;
    }

    .pre-input {
        width: 40%;
        flex: 0 0 40%;
    }

    .pre-regex-box {
        padding-top: 1px;
        padding-left: 1%;
        padding-right: 1%;
        width: 18%;
        flex: 0 0 18%;
        height: 620px;
        overflow-y: auto;
    }

    .pre-view {
        width: 40%;
        flex: 0 0 40%;
    }

    .div-textarea {
        box-shadow: 0 0 0 1px #dcdfe6;
        text-align: left;
        height: 620px;
        overflow-y: auto;
        white-space: pre-wrap;
    }

    .pre-matcher-item {
        padding: 4px;
        box-shadow: 0 0 0 1px #dcdfe6;
        margin-bottom: 4px;
    }


    .b-0 {
        background: #ffdfff;
        box-shadow: 0 0 0 1px #df56ad;
    }


    .b-1 {
        background: #cff5ff;
        box-shadow: 0 0 0 1px #09f;
    }

    div[contenteditable='true']:focus-visible {
        outline: none;
        box-shadow: 0 0 0 1px #09f;
    }
</style>\