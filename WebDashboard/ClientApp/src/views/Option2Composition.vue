

<script setup lang="ts">
    import { computed, ref } from "vue";
    import { json2Obj } from '../utils/jsonStringReader'


    const original = ref("");

    const preview = computed(() => {
        return optionToComposition(original.value);
    });

    // 将选项式 API 转换为组合式 API
    const optionToComposition = (code: string) => {
        // 替换 data() 为 setup()
        if (!code)
            return "";

        const obj = eval("(" + code + ")");
        const props = Object.keys(obj);

        let data = "{ \n<ppp> setup(props, ctx) { \n";

        let dataKeys: string[] = [];
        let propKeys: string[] = [];
        let methodKeys: string[] = [];
        let computedKeys: string[] = [];

        const lifecycleHooksMap: { [key: string]: string } = {
            created: 'onCreated',
            mounted: 'onMounted',
            updated: 'onUpdated',
            destroyed: 'onUnmounted',
        };

        for (var i = 0; i < props.length; i++) {
            const prop = props[i];

            if (prop === "props") {
                const propObj = obj.props;

                propKeys = Object.keys(propObj);
                let finalData: string = (/(props\:((.|\n)*))data/.exec(code)[1]) ?? "";
                data = data.replace("<ppp>", finalData);
                continue;
            }

            if (prop === "watch") {
                const watchObj = obj.watch;

                const watchKeys = Object.keys(watchObj);
                for (var m = 0; m < watchKeys.length; m++) {
                    const watchKey = watchKeys[m];
                    let finalData = `    Vue.watch(${watchKey}, ${watchObj[watchKey].toString()})`;
                    data += finalData + "\n";
                }
                data += "\r";
            }

            if (prop === "data") {
                let dataStr = obj.data.toString().replace(/data\(\)\s*\{(\s|\n)*return\s*((.|\n)*)\}/g, "$2");

                const dataObj = json2Obj(dataStr);

                dataKeys = Object.keys(dataObj);
                for (var m = 0; m < dataKeys.length; m++) {
                    const dataName = dataKeys[m];
                    const dataVal = dataObj[dataName];

                    let finalData = `    const ${dataName} = Vue.ref(${dataVal});`;
                    data += finalData + "\n";
                }
                data += "\r";
            }

            if (lifecycleHooksMap[prop]) {
                const hookObj = obj[prop];

                let hookStr = hookObj.toString();

                for (var dataIndex = 0; dataIndex < dataKeys.length; dataIndex++) {
                    const dataReg = new RegExp(`this\.${dataKeys[dataIndex]}`, 'g');
                    hookStr = hookStr.replace(dataReg, `${dataKeys[dataIndex]}.value`);
                }
                for (var dataIndex = 0; dataIndex < propKeys.length; dataIndex++) {
                    const dataReg = new RegExp(`this\.${propKeys[dataIndex]}`, 'g');
                    hookStr = hookStr.replace(dataReg, `props.${dataKeys[dataIndex]}.value`);
                }


                let finalData = hookStr.replace(/(\w+)\(\)\s*\{((.|\n)*)/g, `    Vue.${lifecycleHooksMap[prop]}(() => {$2);`) + "\n";
                data += finalData;
                data += "\r";
            }

            if (prop === "computed") {
                const computedObj = obj.computed;

                computedKeys = Object.keys(computedObj);
                for (var m = 0; m < computedKeys.length; m++) {
                    const comuptedName = computedKeys[m];
                    let computedStr = computedObj[comuptedName].toString();

                    for (var dataIndex = 0; dataIndex < dataKeys.length; dataIndex++) {
                        const dataReg = new RegExp(`this\.${dataKeys[dataIndex]}`, 'g');
                        computedStr = computedStr.replace(dataReg, `${dataKeys[dataIndex]}.value`);
                    }
                    for (var dataIndex = 0; dataIndex < propKeys.length; dataIndex++) {
                        const dataReg = new RegExp(`this\.${propKeys[dataIndex]}`, 'g');
                        computedStr = computedStr.replace(dataReg, `props.${dataKeys[dataIndex]}.value`);
                    }


                    let finalData = computedStr.replace(/(\w+)\(\)\s*\{((.|\n)*)/g, "    const $1 = Vue.computed(() => {$2);") + "\n";
                    data += finalData;
                }
                data += "\r";
            }

            if (prop === 'methods') {
                const methodObj = obj.methods;

                methodKeys = Object.keys(methodObj);
                for (var m = 0; m < methodKeys.length; m++) {
                    const methodName = methodKeys[m];
                    let methodStr: string = methodObj[methodName].toString();

                    for (var dataIndex = 0; dataIndex < dataKeys.length; dataIndex++) {
                        const dataReg = new RegExp(`this\.${dataKeys[dataIndex]}`, 'g');
                        methodStr = methodStr.replace(dataReg, `${dataKeys[dataIndex]}.value`);
                    }
                    for (var dataIndex = 0; dataIndex < propKeys.length; dataIndex++) {
                        const dataReg = new RegExp(`this\.${propKeys[dataIndex]}`, 'g');
                        methodStr = methodStr.replace(dataReg, `props.${propKeys[dataIndex]}.value`);
                    }

                    let methodData: string = methodStr.replace(/(\w+)\(([^\)]*)\)\s*\{/g, '    const $1 = ($2) => {') + "\n";
                    methodData = methodData.replace(/this\.\$emit\((((?!\)).|\n)*)\)/g, '      ctx.emit($1)');
                    data += methodData;
                }
                data += "\r";
            }
        }
        data += `return {${[...dataKeys, ...propKeys, ...computedKeys, ...methodKeys].join(',')}} \n} \n}`;
        return data;
    };

    const formModel = (obj: any) => {
        if (obj === null || obj === undefined)
            return obj;

        if (obj.toString() !== "[object Object]")
            return obj;

        if (obj instanceof PredefineClass) {
            return obj.v;
        } else {
            var items = Object.keys(obj);
            let o: any = {};
            for (var i = 0; i < items.length; i++) {
                const propName = items[i];
                o[propName] = formModel(obj[propName]);
            }
            return o;
        }
    }

    // 这里前缀的标识用 'FUNCTION_FLAG' 可根据需要自定修改
    const stringify = (obj: any) => {
        return JSON.stringify(obj, (_, v) => {
            if (typeof v === 'function') {
                return `FUNCTION_FLAG ${v}`
            } else {
                return v
            }
        })
    }


    const parse = (jsonStr: string) => {
        return JSON.parse(jsonStr, (_, value) => {
            if (value && typeof value === 'string') {
                return value.indexOf('FUNCTION_FLAG') > -1 ? new Function(`return ${value.replace('FUNCTION_FLAG', '')}`)() : value
            }
            return value
        })
    }

    const readObj = (objString: string) => {

        var obj = eval("(" + objString + ")");

        console.log(obj);
        return obj;
    }

    class PredefineClass {
        v: string;
        constructor(oldModelString: string) {
            this.v = oldModelString;
        }
    }


</script>

<template>
    <div class="preview">
        <div class="pre-input">
            <el-input type="textarea" v-model="original" :rows="30"></el-input>
        </div>

        <div class="pre-regex-box">

        </div>

        <div class="pre-view div-textarea" contenteditable="true" onkeydown="return false;" onpaste="return false;" v-html="preview">
        </div>
    </div>

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
</style>