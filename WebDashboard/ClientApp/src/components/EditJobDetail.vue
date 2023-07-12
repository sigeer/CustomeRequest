<script setup lang="ts">
    import { computed, onMounted, reactive, ref } from 'vue'
    import { useRouter, useRoute } from 'vue-router';
    import { ActionOption } from '../models/actionOption';
    import HttpRequestUnit from '../models/httpRequestUnit';
    import { IStepNode, StepNode } from '../models/stepNode';
    import request from '../utils/request';

    const router = useRouter();
    const route = useRoute();

    const labelPosition = ref('right')

    let formData = ref(new HttpRequestUnit());

    const isEditting = computed(() => {
        return +route.params.id > 0
    });
    const loadData = () => {
        if (route.params.id) {
            request.get(`/api/LoadJobDetail?id=${route.params.id}`).then(res => {
                formData.value = new HttpRequestUnit(res.id, res.name, res.method, res.url, res.httpRequestHeader, res.httpRequestBody, res.steps);
                formData.value.steps.forEach(step => {
                    setStepAction(step);
                });
            })
        }
    }

    const pageData = reactive<{
        actionOptionList: ActionOption[]
    }>({
        actionOptionList: []
    })

    const state = reactive({
        isSubmitting: false,
        isRunning: false
    });

    const getActionOptionList = async () => {
        const res = await request.get('/api/GetActionOptionList');
        pageData.actionOptionList = res.map((x: any) => new ActionOption(x.id, x.name, x.fromType, x.returnType, x.params));
    }

    const setStepAction = (step: IStepNode) => {
        step.setActionOption(pageData.actionOptionList);
    }

    onMounted(async () => {
        await getActionOptionList();
        loadData();
    })

    const submit = () => {
        state.isSubmitting = true;
        request.post('/api/AddOrUpdateJobDetail', formData.value).then((res: boolean) => {
            if (res) {
                loadData();
            }
        }).finally(() => {
            state.isSubmitting = false;
        })
    }

    const jobResultList = ref<string[]>([]);
    const run = () => {
        if (route.params.id) {
            state.isRunning = true;
            request.get(`/api/RunJobDetail?id=${route.params.id}`).then(res => {
                if (typeof (res) === 'string') {
                    jobResultList.value = [res];
                } else
                jobResultList.value = res;
            }).finally(() => {
                state.isRunning = false;
            })
        }

    }

    const back = () => {
        router.back();
    }

    const newStep = () => {
        formData.value.steps.push(new StepNode());
    }

    const removeStep = (stepIndex: number) => {
        formData.value.steps.splice(stepIndex, 1);
    }

    const getLastReturnType = (stepIndex: number): string => {
        return stepIndex === 0 ? "String" : (formData.value.steps[stepIndex - 1].actionOption?.returnType ?? "");
    }

    const filterAllowedOptions = (stepIndex: number): ActionOption[] => {
        return pageData.actionOptionList.filter(x => x.fromType === getLastReturnType(stepIndex));
    }

</script>

<template>
    <el-form :label-position="labelPosition"
             label-width="100px"
             :model="formData">
        <input type="hidden" v-model="formData.id" autocomplete="off" />
        <el-form-item label="名称">
            <el-input v-model="formData.name" />
        </el-form-item>

        <el-form-item label="Url">
            <el-input v-model="formData.url" />
        </el-form-item>

        <el-form-item label="Method">
            <el-select v-model="formData.method">
                <el-option value="GET">GET</el-option>
                <el-option value="POST">POST</el-option>
            </el-select>
        </el-form-item>

        <el-form-item label="Headers">
            <template v-for="(item, index) in formData.httpRequestHeader">
                <el-col :span="6" >
                    <el-input v-model="item.key" />
                </el-col>
                <el-col :span="2" class="text-center">
                    <span class="text-gray-500">-</span>
                </el-col>
                <el-col :span="6">
                    <el-input v-model="item.value" />
                </el-col>
                <el-col :span="10" style="text-align: left">
                    <a @click="formData.removeHeader(index)">移除</a>
                </el-col>
                
            </template>
            <a @click="formData.addHeader()">添加</a>
        </el-form-item>

        <el-form-item label="Body" v-if="formData.method === 'POST'">
            <el-input v-model="formData.httpRequestBody" type="textarea" placeholder="JSON" />
        </el-form-item>

        <template v-for="(x, stepIndex) in formData.steps">
            <div class="will-input-type">输入类型:{{getLastReturnType(stepIndex)}}</div>
            <el-card shadow="never" style="margin-bottom: 2rem;">

                <el-form-item label="操作">
                    <el-select v-model="x.actionOptionId" @change="setStepAction(x)">
                        <el-option v-for="option in filterAllowedOptions(stepIndex)" :label="option.name" :value="option.id" :key="option.id"></el-option>
                    </el-select>
                </el-form-item>

                <a @click="removeStep(stepIndex)">移除</a>

                <template v-if="x.actionOption">
                    <div class="data-type-info">接受类型:{{x.actionOption.fromType}}</div>

                    <el-form-item v-for="(_, pIndex) in x.actionOption.params" :label="x.actionOption.params[pIndex]">
                        <el-input v-model="x.params[pIndex]"></el-input>
                    </el-form-item>

                    <div class="data-type-info">输出类型:{{x.actionOption.returnType}}</div>

                </template>
            </el-card>
        </template>


        <a @click="newStep">增加处理节点</a>

        <el-form-item>
            <el-button @click="submit" :loading="state.isSubmitting">提交</el-button>
            <el-button @click="run" v-if="isEditting" :loading="state.isRunning">运行</el-button>
            <el-button @click="back">返回</el-button>
        </el-form-item>

        <el-divider>请求结果</el-divider>

        <el-card v-for="jobResult in jobResultList">
            <el-input type="textarea" :value="jobResult" readonly :rows="6">
            </el-input>
        </el-card>
    </el-form>

</template>

<style scoped>
    .will-input-type {
        padding: 12px;
        color: #07a590;
    }

    .data-type-info {
        margin: 18px 0px;
    }
</style>
