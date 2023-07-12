export class HttpRequestHeaderItem {
    key: string;
    value: string;

    constructor(key: string = "", value: string = "") {
        this.key = key;
        this.value = value;
    }
}