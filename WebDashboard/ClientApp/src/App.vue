<template>
    <el-container>
        <el-header>
            <el-space :size="10">
                {{ route.meta.title }}
                <router-link :to="{ path: '/'}">计划任务列表</router-link>
                <router-link :to="{ path: '/jobdetail/list'}">可执行任务列表</router-link>
                <a href="http://localhost:7153/hangfire" target="_blank">hangfire</a>
            </el-space>

        </el-header>
        <el-main>
            <router-view></router-view>
        </el-main>
    </el-container>
</template>

<script setup lang="ts">
    import { useRoute } from 'vue-router';

    const route = useRoute();

</script>