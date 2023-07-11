<script setup lang="ts">
    import { reactive, ref, onMounted } from 'vue'
    import { useRouter, useRoute } from 'vue-router';
    import request from '../utils/request';
    import type { FormInstance, FormRules } from 'element-plus';


    const router = useRouter();
    const route = useRoute();

    const labelPosition = ref('right')

    const formData = ref<{
        jobStoreId: string | null,
        jobId: string | null,
        name: '',
        cron: '',
        jobDetailId: number | null,
        remark: string | null
    }>({
        jobStoreId: null,
        jobId: null,
        name: '',
        cron: '',
        jobDetailId: null,
        remark: null
    });

    const formRules = reactive<FormRules<{
        jobStoreId: string | null,
        jobId: string | null,
        name: '',
        cron: '',
        jobDetailId: number | null,
        remark: string | null
    }>>({
        name: [{ required: true, message: "name为必填项" }],
        jobId: [{ required: true, message: "jobId为必填项" }],
        cron: [{ required: true, message: "cron为必填项" }]
    })

    const state = reactive({
        isSubmitting: false
    });

    const pageData = reactive<{ jobDetailList: any[] }>({
        jobDetailList: []
    })

    const loadData = async () => {
        if (route.params.id) {
            formData.value = await request.get(`/api/LoadJob?id=${route.params.id}`);
        }
    }

    onMounted(() => {
        loadData()
    });


    const submit = () => {
        ruleFormRef.value?.validate((valid, fields) => {
            if (valid) {
                request.post('/api/AddOrUpdateJob', formData.value).then(() => {
                    back();
                });
            } else {
                console.log('error submit!', fields)
            }
        })

    }

    const getJobDetailList = async () => {
        pageData.jobDetailList = await (await fetch("/api/GetJobDetailList")).json();
    }

    onMounted(() => {
        getJobDetailList();
    })

    const back = () => {
        router.back();
    }

    const guid = (): string => {
        function S4() {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        }
        return (
            S4() +
            S4() +
            "-" +
            S4() +
            "-" +
            S4() +
            "-" +
            S4() +
            "-" +
            S4() +
            S4() +
            S4()
        );
    }

    const generateGuid = () => {
        formData.value.jobId = guid();
    }

    const ruleFormRef = ref<FormInstance>();
</script>

<template>
    <el-form :label-position="labelPosition"
             ref="ruleFormRef"
             label-width="100px"
             :rules="formRules"
             :model="formData">
        <input type="hidden" v-model="formData.jobStoreId" autocomplete="off" />
        <el-form-item label="JobId" prop="jobId">
            <el-input v-model="formData.jobId" />
            <a @click="generateGuid">随机生成</a>
        </el-form-item>
        <el-form-item label="名称" prop="name">
            <el-input v-model="formData.name" />
        </el-form-item>
        <el-form-item label="Cron表达式" prop="cron">
            <el-input v-model="formData.cron" />
        </el-form-item>
        <el-form-item label="选择任务">
            <el-select v-model="formData.jobDetailId">
                <el-option v-for="x in pageData.jobDetailList" :value="x.id" :label="x.name">{{ x.name }}</el-option>
            </el-select>
            <router-link v-if="formData.jobDetailId" :to="{ path: '/jobdetail/edit/' + formData.jobDetailId }" target="_blank">查看详细</router-link>
        </el-form-item>
        <el-form-item label="备注">
            <el-input v-model="formData.remark" type="textarea" placeholder="备注（非必填）" />
        </el-form-item>

        <el-form-item>
            <el-button @click="submit" :loading="state.isSubmitting">提交</el-button>
            <el-button @click="back">返回</el-button>
        </el-form-item>
    </el-form>

    <el-row>
        <div class="alert">
            <h2>
                cron表达式详解
            </h2>

            <p>
                corn从左到右（用空格隔开）：秒 分 小时 月份中的日期 月份 星期中的日期 年份（可选）
            </p>

            <p>
                每一个域都使用数字或特殊字符：
            </p>
            <p>
                <span class="text-info">（1）*：表示匹配该域的任意值。</span>假如在Minutes域使用*, 即表示每分钟都会触发事件。
            </p>
            <p>
                <span class="text-info">（2）?：只能用在DayofMonth和DayofWeek两个域。它也匹配域的任意值，但实际不</span>会。因为DayofMonth和DayofWeek会相互影响。例如想在每月的20日触发调度，不管20日到底是星期几，则只能使用如下写法：
                13 13 15 20 * ?, 其中最后一位只能用？，而不能使用*，如果使用*表示不管星期几都会触发，实际上并不是这样。
            </p>
            <p>
                <span class="text-info">（3）-：表示范围。</span>例如在Minutes域使用5-20，表示从5分到20分钟每分钟触发一次&nbsp;
            </p>
            <p>
                <span class="text-info">（4）/：表示起始时间开始触发，然后每隔固定时间触发一次。</span>例如在Minutes域使用5/20,则意味着5分钟触发一次，而25，45等分别触发一次.&nbsp;
            </p>
            <p>
                <span class="text-info">（5）,：表示列出枚举值。</span>例如：在Minutes域使用5,20，则意味着在5和20分每分钟触发一次。&nbsp;
            </p>
            <p>
                <span class="text-info">（6）L：表示最后，只能出现在DayofWeek和DayofMonth域。</span>如果在DayofWeek域使用5L,意味着在最后的一个星期四触发。&nbsp;
            </p>
            <p>
                <span class="text-info">（7）W:表示有效工作日(周一到周五),只能出现在DayofMonth域，系统将在离指定日期的最近的有效工作日触发事件。</span>例如：在
                DayofMonth使用5W，如果5日是星期六，则将在最近的工作日：星期五，即4日触发。如果5日是星期天，则在6日(周一)触发；如果5日在星期一到星期五中的一天，则就在5日触发。另外一点，W的最近寻找不会跨过月份 。
            </p>
            <p>
                <span class="text-info">（8）LW:这两个字符可以连用，表示在某个月最后一个工作日，即最后一个星期五。</span>&nbsp;
            </p>
            <p>
                <span class="text-info">（9）#:用于确定每个月第几个星期几，只能出现在DayofWeek域。</span>例如在4#2，表示某月的第二个星期三。
            </p>

            <p>
                <strong><span>常用例子</span></strong>
            </p>
            <p>
                <span>（0）<strong><span class="text-info">0/20 * * * * ?</span></strong>&nbsp;&nbsp;</span>&nbsp;表示每20秒
            </p>


            <p>
                （1）<strong><span class="text-info">0 0 2 1 * ?</span></strong>&nbsp;&nbsp;&nbsp;表示在每月的1日的凌晨2点调整任务
            </p>
            <p>
                （2）<strong><span class="text-info">0 15 10 ? * MON-FRI</span>&nbsp;</strong>&nbsp;
                表示周一到周五每天上午10:15执行作业
            </p>
            <p>
                （3）<strong><span class="text-info">0 15 10 ? 6L 2002-2006</span></strong>&nbsp;&nbsp;
                表示2002-2006年的每个月的最后一个星期五上午10:15执行作
            </p>
            <p>
                （4）<strong><span class="text-info">0 0 10,14,16 * * ?</span></strong>&nbsp;&nbsp;&nbsp;每天上午10点，下午2点，4点&nbsp;
            </p>
            <p>
                （5）<strong><span class="text-info">0 0/30 9-17 * * ?</span></strong>&nbsp;&nbsp;
                朝九晚五工作时间内每半小时&nbsp;
            </p>
            <p>
                （6）<strong><span class="text-info">0 0 12 ? * WED</span></strong>&nbsp;&nbsp; &nbsp;表示每个星期三中午12点&nbsp;
            </p>
            <p>
                （7）<strong><span class="text-info">0 0 12 * * ?</span></strong>&nbsp;&nbsp;&nbsp;每天中午12点触发&nbsp;
            </p>
            <p>
                （8）<strong><span class="text-info">0 15 10 ? * * &nbsp;</span></strong>&nbsp;&nbsp;每天上午10:15触发&nbsp;
            </p>
            <p>
                （9）<strong><span class="text-info">0 15 10 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                每天上午10:15触发&nbsp;
            </p>
            <p>
                （10）<strong><span class="text-info">0 15 10 * * ? *</span>&nbsp;</strong>&nbsp; &nbsp;每天上午10:15触发&nbsp;
            </p>
            <p>
                （11）<strong><span class="text-info">0 15 10 * * ? 2005</span></strong>&nbsp;&nbsp; &nbsp;2005年的每天上午10:15触发&nbsp;
            </p>
            <p>
                （12）<strong><span class="text-info">0 * 14 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                在每天下午2点到下午2:59期间的每1分钟触发&nbsp;
            </p>
            <p>
                （13）<strong><span class="text-info">0 0/5 14 * * ?</span></strong>&nbsp;&nbsp; &nbsp;在每天下午2点到下午2:55期间的每5分钟触发&nbsp;
            </p>
            <p>
                （14）<strong><span class="text-info">0 0/5 14,18 * * ?</span></strong>&nbsp;&nbsp; &nbsp;
                在每天下午2点到2:55期间和下午6点到6:55期间的每5分钟触发&nbsp;
            </p>
            <p>
                （15）<strong><span class="text-info">0 0-5 14 * * ?</span>&nbsp;</strong>&nbsp; &nbsp;在每天下午2点到下午2:05期间的每1分钟触发&nbsp;
            </p>
            <p>
                （16）<strong><span class="text-info">0 10,44 14 ? 3 WED</span></strong>&nbsp;&nbsp; &nbsp;每年三月的星期三的下午2:10和2:44触发&nbsp;
            </p>
            <p>
                （17）<strong><span class="text-info">0 15 10 ? * MON-FRI</span>&nbsp;</strong>&nbsp; &nbsp;周一至周五的上午10:15触发&nbsp;
            </p>
            <p>
                （18）<strong><span class="text-info">0 15 10 15 * ?</span>&nbsp;</strong>&nbsp; &nbsp;每月15日上午10:15触发&nbsp;
            </p>
            <p>
                （19）<strong><span class="text-info">0 15 10 L * ?</span>&nbsp;</strong>&nbsp; &nbsp;每月最后一日的上午10:15触发&nbsp;
            </p>
            <p>
                （20）<strong><span class="text-info">0 15 10 ? * 6L</span>&nbsp;</strong>&nbsp; &nbsp;每月的最后一个星期五上午10:15触发&nbsp;
            </p>
            <p>
                （21）<strong><span class="text-info">0 15 10 ? * 6L 2002-2005</span></strong>&nbsp;&nbsp;
                2002年至2005年的每月的最后一个星期五上午10:15触发&nbsp;
            </p>
            <p>
                （22）<strong><span class="text-info">0 15 10 ? * 6#3</span></strong>&nbsp;&nbsp;
                每月的第三个星期五上午10:15触发
            </p>
        </div>
    </el-row>
</template>


<style scoped>
    .alert p {
        text-align: left;
    }
</style>
