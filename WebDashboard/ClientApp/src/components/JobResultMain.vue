<script setup lang="ts">
    import { reactive, onMounted } from 'vue'
    import { useRoute, useRouter } from 'vue-router';
    import request from '../utils/request';

    const route = useRoute();
    const router = useRouter();


    const tableModel = reactive({
        dataSource: []
    });

    onMounted(() => {
        getDataSource();
    });


    const getDataSource = () => {
        if (route.params.id) {
            request.get(`/api/GetJobResultList?jobDetailId=${route.params.id}`).then(res => {
                tableModel.dataSource = res;
            })
        } else {
            router.back();

        }

    }
</script>

<template>
    <el-table :data="tableModel.dataSource" style="width: 100%">
        <el-table-column prop="creationTime" label="更新时间" width="280" />
        <el-table-column type="expand" label="内容" width="280">
            <template #default="scope">
                {{scope.row.result}}
            </template>
        </el-table-column>
        <el-table-column label="简略">
            <template #default="scope">
                {{scope.row.result?.substring(0, 100)}}
            </template>
        </el-table-column>
    </el-table>
</template>

