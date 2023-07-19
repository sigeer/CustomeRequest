import { createRouter, createWebHashHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

import JobMain from '../components/JobMain.vue'
import NewJob from '../components/EditJob.vue'
import JobDetailMain from '../components/JobDetailMain.vue'
import EditJobDetail from '../components/EditJobDetail.vue'
import JobResultMain from '../components/JobResultMain.vue'
import RegexMatch from '../views/RegexMatch.vue'

const routes: Array<RouteRecordRaw> =[
    { path: '/', component: JobMain, meta: { title: '任务列表'} },
    { path: '/job/new', component: NewJob, meta: { title: '新任务' } },
    { path: '/job/edit/:id', component: NewJob, meta: { title: '修改任务' } },
    { path: '/jobdetail/list', component: JobDetailMain, meta: { title: '请求列表' } },
    { path: '/jobdetail/new', component: EditJobDetail, meta: { title: '新请求' } },
    { path: '/jobdetail/edit/:id', component: EditJobDetail, meta: { title: '修改请求' } },
    { path: '/jobresult/:id', component: JobResultMain, meta: { title: '修改请求' } },
    { path: '/regex', component: RegexMatch, meta: { title: '文本替换' } }
]

export const router = createRouter({
    // 4. 内部提供了 history 模式的实现。为了简单起见，我们在这里使用 hash 模式。
    history: createWebHashHistory(),
    routes, // `routes: routes` 的缩写
})


