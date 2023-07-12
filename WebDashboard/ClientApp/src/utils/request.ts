export default class request {
    static getEmpty(url: string) {
        return fetch(url);
    }

    static get(url: string) {
        return fetch(url).then(response => {
            if (response.ok) {
                if (response.headers?.get('Content-Type')?.includes('application/json')) {
                    return response.json(); // 返回 JSON 格式的响应
                } else {
                    return response.text(); // 返回文本响应
                }
            } else {
                throw new Error('Network response was not ok.');
            }
        });
    }

    static post(url: string, data: any) {
        return fetch(url, {
            method: 'post',
            body: JSON.stringify(data),
            headers: {
                "Content-Type": "application/json"
            }
        }).then(res => res.json());
    }

    static delete(url: string) {
        return fetch(url, {
            method: 'delete'
        }).then(res => res.json());
    }

    static put(url: string, data: any) {
        return fetch(url, {
            method: 'put',
            body: JSON.stringify(data),
            headers: {
                "Content-Type": "application/json"
            }
        }).then(res => res.json());
    }
}