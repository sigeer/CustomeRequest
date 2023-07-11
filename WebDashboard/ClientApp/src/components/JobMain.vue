<script setup lang="ts">
    import { reactive, onMounted } from 'vue'
    import request from '../utils/request';

    const tableModel = reactive({
        dataSource: []
    });

    onMounted(() => {
        getDataSource();
    });


    const getDataSource = () => {
        request.get(`/api/GetJobList`).then(res => {
            tableModel.dataSource = res;
        })
    }

    const showDeleteConfirm = (id: number) => {
        if (confirm(`确定删除`)) {
            request.delete(`/api/DeleteJob?id=${id}`).then(() => {
                getDataSource();
            })
        }
    }

    const setStatus = (jobStoreId: number, close: boolean) => {
        let url = close ? '/api/joboff' : '/api/jobon';
        request.getEmpty(`${url}?id=${jobStoreId}`).then(() => {
            getDataSource();
        })

    }
</script>

<template>
    <el-table :data="tableModel.dataSource" style="width: 100%">
        <el-table-column prop="jobId" label="JobId" width="180" />
        <el-table-column prop="name" label="任务名" width="180" />
        <el-table-column prop="cron" label="Cron" width="180" />
        <el-table-column prop="remark" label="备注" />
        <el-table-column label="状态" width="180">
            <template #default="scope">
                <span>{{ scope.row.status === 1 ? '运行中' : '已关闭' }}</span>
                <a @click="setStatus(scope.row.jobStoreId, scope.row.status === 1)">{{ scope.row.status === 1 ? '停止' : '启动' }}</a>
            </template>
        </el-table-column>
        <el-table-column width="120">
            <template #header>
                <a class="link">
                    <router-link :to="{ path: '/job/new' }">
                        添加
                    </router-link>
                </a>
            </template>

            <template #default="scope">
                <router-link :to="{ path: '/job/edit/' + scope.row.jobStoreId }"> 编辑</router-link>
                <a @click="showDeleteConfirm(scope.row.jobStoreId)" class="link">
                    删除
                </a>
            </template>
        </el-table-column>
    </el-table>
</template>

