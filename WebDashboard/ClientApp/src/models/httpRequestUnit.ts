import { HttpRequestHeaderItem } from "./httpRequestHeader";
import { IStepNode, StepNode } from "./stepNode";

export default class HttpRequestUnit {
    id: number;
    name: string;
    method: string = 'GET';
    url: string;
    httpRequestHeader: HttpRequestHeaderItem[];
    httpRequestBody: string | null;
    steps: IStepNode[];
    constructor(id: number = 0, name: string = '', method: string = 'GET', url: string = '', httpRequestHeader: HttpRequestHeaderItem[] = [], httpRequestBody: string | null = null, steps: any[] = []) {
        this.id = id;
        this.name = name;
        this.method = method;
        this.url = url;
        this.httpRequestBody = httpRequestBody;
        this.httpRequestHeader = httpRequestHeader;
        this.steps = steps.map((x: any) => new StepNode(x));
    }

    //getPostModel(): any {
    //    return {
    //        id: this.id,
    //        name: this.name,
    //        method: this.method,
    //        url: this.url,
    //        httpRequestBody: this.httpRequestBody,
    //        httpRequestHeader: this.httpRequestHeader,
    //        stepJson: JSON.stringify(this.steps)
    //    }
    //}

    validate(): boolean {
        if (this.steps.length > 0)
            return this.steps[this.steps.length - 1].actionOption?.returnType === 'String';

        return true;
    }

    removeHeader(index: number) {
        this.httpRequestHeader.splice(index, 1);
    }

    addHeader() {
        this.httpRequestHeader.push(new HttpRequestHeaderItem())
    }
}