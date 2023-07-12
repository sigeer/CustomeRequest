import { ref } from 'vue';

export function useHttpRequestUnit() {
    const state = ref<{
        jobStoreId: null | number,
        jobId: null | number,
        name: string,
        cron: string,
        jobDetailId: null | number,
        remark: null | string,
        steps: any[]
    }>({
        jobStoreId: null,
        jobId: null,
        name: '',
        cron: '',
        jobDetailId: null,
        remark: null,
        steps: []
    });

    return state;
}
