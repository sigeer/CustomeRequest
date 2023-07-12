<script setup lang="ts">
    import { reactive, onMounted } from 'vue'
import request from '../utils/request';

    const tableModel = reactive({
        dataSource: []
    });

    onMounted(() => {
        getDataSource();
    });


    const getDataSource = async () => {
        tableModel.dataSource = await (await fetch("/api/GetJobDetailList")).json();
    }

    const showDeleteConfirm = (id: number) => {
        if (confirm(`确定删除`)) {
            request.delete(`/api/DeleteJobDetail?id=${id}`).then(() => {
                getDataSource();
            })
        }
    }
</script>

<template>
    <el-table :data="tableModel.dataSource" style="width: 100%">
        <el-table-column prop="name" label="名称" width="180" />
        <el-table-column prop="url" label="Url" />
        <el-table-column prop="method" label="Method" width="180" />
        <el-table-column label="请求记录" width="120">
            <template #default="scope">
                <router-link :to="{ path: '/jobresult/' + scope.row.id }"> 查看</router-link>
            </template>
        </el-table-column>

        <el-table-column width="120">
            <template #header>
                <router-link :to="{ path: '/jobdetail/new' }">
                    添加
                </router-link>
            </template>

            <template #default="scope">
                <router-link :to="{ path: '/jobdetail/edit/' + scope.row.id }"> 编辑</router-link>
                <a @click="showDeleteConfirm(scope.row.id)" class="link">
                    删除
                </a>
            </template>
        </el-table-column>
    </el-table>
</template>

